using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class SystemSettingsRepository : ISystemSettingsService
    {
        private readonly TesterLabDbContext _context;
        private readonly ILogger<SystemSettingsRepository> _logger;
        private readonly IConfiguration _configuration;

        public SystemSettingsRepository(
            TesterLabDbContext context,
            ILogger<SystemSettingsRepository> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<SystemSettingsViewModel> GetAllSettingsAsync()
        {
            var settings = new SystemSettingsViewModel();

            // Charger tous les paramètres depuis la base
            var dbSettings = await _context.SystemSettings.ToListAsync();

            // Mapper vers le ViewModel
            settings.General = await LoadCategoryAsync<GeneralSettings>("General", dbSettings);
            settings.Email = await LoadCategoryAsync<EmailSettings>("Email", dbSettings);
            settings.Testing = await LoadCategoryAsync<TestingSettings>("Testing", dbSettings);
            settings.Security = await LoadCategoryAsync<SecuritySettings>("Security", dbSettings);
            settings.Branding = await LoadCategoryAsync<BrandingSettings>("Branding", dbSettings);
            settings.Notifications = await LoadCategoryAsync<NotificationSettings>("Notifications", dbSettings);
            settings.Storage = await LoadCategoryAsync<StorageSettings>("Storage", dbSettings);

            return settings;
        }

        public async Task<T> GetSettingAsync<T>(string key, T defaultValue = default)
        {
            try
            {
                var setting = await _context.SystemSettings
                    .FirstOrDefaultAsync(s => s.Key == key);

                if (setting == null)
                    return defaultValue;

                // Convertir selon le type
                return ConvertValue<T>(setting.Value, setting.DataType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture du paramètre {Key}", key);
                return defaultValue;
            }
        }

        public async Task SetSettingAsync(string key, object value, string category = "General", string? updatedBy = null)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == key);

            var valueString = value?.ToString() ?? string.Empty;

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    Key = key,
                    Value = valueString,
                    Category = category,
                    DataType = DetermineDataType(value),
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = updatedBy
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = valueString;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = updatedBy;
                _context.SystemSettings.Update(setting);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Paramètre {Key} mis à jour par {User}", key, updatedBy ?? "System");
        }

        public async Task SaveSettingsAsync(SystemSettingsViewModel settings, string? updatedBy = null)
        {
            try
            {
                // Sauvegarder chaque catégorie
                await SaveCategoryAsync(settings.General, "General", updatedBy);
                await SaveCategoryAsync(settings.Email, "Email", updatedBy);
                await SaveCategoryAsync(settings.Testing, "Testing", updatedBy);
                await SaveCategoryAsync(settings.Security, "Security", updatedBy);
                await SaveCategoryAsync(settings.Branding, "Branding", updatedBy);
                await SaveCategoryAsync(settings.Notifications, "Notifications", updatedBy);
                await SaveCategoryAsync(settings.Storage, "Storage", updatedBy);

                _logger.LogInformation("Tous les paramètres système sauvegardés par {User}", updatedBy ?? "System");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres");
                throw;
            }
        }

        public async Task<bool> TestEmailSettingsAsync(EmailSettings settings)
        {
            try
            {
                using var client = new System.Net.Mail.SmtpClient(settings.SmtpHost, settings.SmtpPort);
                client.EnableSsl = settings.EnableSsl;
                client.Credentials = new System.Net.NetworkCredential(
                    settings.SmtpUsername,
                    settings.SmtpPassword);

                var message = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(settings.FromEmail, settings.FromName),
                    Subject = "Test de configuration SMTP",
                    Body = "Ceci est un email de test. La configuration SMTP fonctionne correctement.",
                    IsBodyHtml = false
                };

                message.To.Add(settings.FromEmail); // Envoyer à soi-même pour tester

                await client.SendMailAsync(message);

                _logger.LogInformation("Test SMTP réussi");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec du test SMTP");
                return false;
            }
        }

        public async Task ResetToDefaultsAsync(string category)
        {
            var settings = await _context.SystemSettings
                .Where(s => s.Category == category)
                .ToListAsync();

            _context.SystemSettings.RemoveRange(settings);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Paramètres de la catégorie {Category} réinitialisés", category);
        }

        // ═══════════════════════════════════════════════════════
        // MÉTHODES PRIVÉES
        // ═══════════════════════════════════════════════════════

        private async Task<T> LoadCategoryAsync<T>(string category, List<SystemSetting> dbSettings) where T : new()
        {
            var instance = new T();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var key = $"{category}.{prop.Name}";
                var setting = dbSettings.FirstOrDefault(s => s.Key == key);

                if (setting != null)
                {
                    try
                    {
                        var convertedValue = ConvertValue(setting.Value, prop.PropertyType);
                        prop.SetValue(instance, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossible de convertir {Key}", key);
                    }
                }
            }

            return instance;
        }

        private async Task SaveCategoryAsync<T>(T settings, string category, string? updatedBy)
        {
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var key = $"{category}.{prop.Name}";
                var value = prop.GetValue(settings);

                if (value != null)
                {
                    await SetSettingAsync(key, value, category, updatedBy);
                }
            }
        }

        private T ConvertValue<T>(string value, string dataType)
        {
            try
            {
                return dataType switch
                {
                    "Boolean" => (T)(object)bool.Parse(value),
                    "Integer" => (T)(object)int.Parse(value),
                    "Double" => (T)(object)double.Parse(value),
                    "Json" => JsonSerializer.Deserialize<T>(value),
                    _ => (T)Convert.ChangeType(value, typeof(T))
                };
            }
            catch
            {
                return default;
            }
        }

        private object ConvertValue(string value, Type targetType)
        {
            if (targetType == typeof(bool))
                return bool.Parse(value);
            if (targetType == typeof(int))
                return int.Parse(value);
            if (targetType == typeof(double))
                return double.Parse(value);

            return Convert.ChangeType(value, targetType);
        }

        private string DetermineDataType(object value)
        {
            return value switch
            {
                bool => "Boolean",
                int => "Integer",
                double => "Double",
                _ => "String"
            };
        }
    }
}

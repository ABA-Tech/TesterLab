using TesterLab.Domain.Models;

namespace TesterLab.Domain.interfaces.Services
{
    public interface ISystemSettingsService
    {
        Task<SystemSettingsViewModel> GetAllSettingsAsync();
        Task<T> GetSettingAsync<T>(string key, T defaultValue = default);
        Task SetSettingAsync(string key, object value, string category = "General", string? updatedBy = null);
        Task SaveSettingsAsync(SystemSettingsViewModel settings, string? updatedBy = null);
        Task<bool> TestEmailSettingsAsync(EmailSettings settings);
        Task ResetToDefaultsAsync(string category);
    }
}

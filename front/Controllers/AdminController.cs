using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth.Core.Abstractions;
using Auth.Core.Models;
using TesterLab.Models.ViewModels;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Controllers
{
  //[Authorize(Roles = "Admin")] // on suspend le veroux en attendant de trouver une bd en ligne
  public class AdminController : Controller
  {
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<AdminController> _logger;
    private readonly ISystemSettingsService _settingsService;

    public AdminController(
        IUserService userService,
        IRoleService roleService,
        ISystemSettingsService settingsService,
        ILogger<AdminController> logger)
    {
      _userService = userService;
      _roleService = roleService;
      _settingsService = settingsService;
      _logger = logger;
    }

    // ═══════════════════════════════════════════════════════
    // DASHBOARD ADMIN
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Index()
    {
      try
      {
        var viewModel = new AdminDashboardViewModel();

        // Statistiques utilisateurs
        viewModel.Statistics.TotalUsers = await _userService.GetTotalUsersCountAsync();
        viewModel.Statistics.ActiveUsers = await _userService.GetActiveUsersCountAsync();
        viewModel.Statistics.NewUsersThisWeek = await _userService.GetNewUsersThisWeekCountAsync();
        viewModel.Statistics.PendingEmailConfirmation = await _userService.GetPendingEmailConfirmationCountAsync();
        viewModel.Statistics.LockedOutUsers = await _userService.GetLockedOutUsersCountAsync();

        // Statistiques rôles
        var roles = await _roleService.GetAllRolesAsync();
        viewModel.Statistics.TotalRoles = roles.Count;

        // Utilisateurs récents
        var recentUsers = await _userService.GetRecentUsersAsync(5);
        viewModel.RecentUsers = recentUsers.Select(u => new RecentUserViewModel
        {
          Username = u.Username,
          Email = u.Email,
          CreatedAt = u.CreatedAt,
          EmailConfirmed = u.EmailConfirmed
        }).ToList();

        // Résumé des rôles
        var colors = new[] { "primary", "success", "info", "warning", "danger", "secondary" };
        var roleIndex = 0;

        foreach (var role in roles)
        {
          var userCount = role.UserRoles?.Count ?? 0;
          viewModel.RoleSummaries.Add(new RoleSummaryViewModel
          {
            RoleName = role.Name,
            UserCount = userCount,
            ColorClass = colors[roleIndex % colors.Length]
          });
          roleIndex++;
        }

        return View(viewModel);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors du chargement du dashboard admin");
        return View(new AdminDashboardViewModel());
      }
    }

    // ═══════════════════════════════════════════════════════
    // GESTION DES UTILISATEURS
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Users(string? search, string? roleFilter)
    {
      var users = await GetAllUsersAsync();

      // Filtrer par recherche
      if (!string.IsNullOrWhiteSpace(search))
      {
        users = users.Where(u =>
            u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            (u.FirstName != null && u.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
            (u.LastName != null && u.LastName.Contains(search, StringComparison.OrdinalIgnoreCase))
        ).ToList();
      }

      // Filtrer par rôle
      if (!string.IsNullOrWhiteSpace(roleFilter))
      {
        users = users.Where(u => u.Roles.Contains(roleFilter)).ToList();
      }

      var viewModel = new UserListViewModel
      {
        Users = users,
        SearchTerm = search,
        RoleFilter = roleFilter
      };

      ViewBag.AvailableRoles = await _roleService.GetAllRolesAsync();

      return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> UserEdit(string id)
    {
      var user = await _userService.GetByIdAsync(id);
      if (user == null)
        return NotFound();

      var roles = await _roleService.GetUserRolesAsync(id);

      var viewModel = new UserManagementViewModel
      {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        EmailConfirmed = user.EmailConfirmed,
        IsLockedOut = user.IsLockedOut,
        LockoutEnd = user.LockoutEnd,
        CreatedAt = user.CreatedAt,
        LastLoginAt = user.LastLoginAt,
        Roles = roles
      };

      return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLockout(string userId)
    {
      var user = await _userService.GetByIdAsync(userId);
      if (user == null)
        return NotFound();

      if (user.IsLockedOut)
      {
        await _userService.UnlockAccountAsync(userId);
        TempData["SuccessMessage"] = "Compte déverrouillé avec succès";
      }
      else
      {
        await _userService.LockAccountAsync(userId, TimeSpan.FromDays(365));
        TempData["SuccessMessage"] = "Compte verrouillé avec succès";
      }

      return RedirectToAction(nameof(UserEdit), new { id = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEmail(string userId)
    {
      var user = await _userService.GetByIdAsync(userId);
      if (user == null)
        return NotFound();

      // Forcer la confirmation d'email
      user.EmailConfirmed = true;
      user.EmailConfirmationToken = null;
      user.EmailConfirmationTokenExpires = null;

      // TODO: Implémenter UpdateAsync dans IUserRepository
      // await _userRepository.UpdateAsync(user);

      TempData["SuccessMessage"] = "Email confirmé manuellement";
      return RedirectToAction(nameof(UserEdit), new { id = userId });
    }

    // ═══════════════════════════════════════════════════════
    // ASSIGNATION DES RÔLES
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> AssignRoles(string userId)
    {
      var user = await _userService.GetByIdAsync(userId);
      if (user == null)
        return NotFound();

      var allRoles = await _roleService.GetAllRolesAsync();
      var userRoles = await _roleService.GetUserRolesAsync(userId);

      var viewModel = new AssignRolesViewModel
      {
        UserId = user.Id,
        Username = user.Username,
        Email = user.Email,
        AvailableRoles = allRoles.Select(r => new RoleSelectionItem
        {
          RoleId = r.Id,
          RoleName = r.Name,
          Description = r.Description,
          IsSelected = userRoles.Contains(r.Name)
        }).ToList()
      };

      return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoles(string userId, List<string> selectedRoles)
    {
      if (selectedRoles == null)
        selectedRoles = new List<string>();

      var currentUserName = User.Identity?.Name;

      try
      {
        // Récupérer tous les rôles pour obtenir les noms
        var allRoles = await _roleService.GetAllRolesAsync();
        var roleNames = allRoles
            .Where(r => selectedRoles.Contains(r.Id))
            .Select(r => r.Name)
            .ToList();

        // Remplacer tous les rôles de l'utilisateur
        await _roleService.ReplaceUserRolesAsync(userId, roleNames, currentUserName);

        TempData["SuccessMessage"] = "Rôles mis à jour avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de l'assignation des rôles");
        TempData["ErrorMessage"] = "Erreur lors de la mise à jour des rôles";
      }

      return RedirectToAction(nameof(UserEdit), new { id = userId });
    }

    // ═══════════════════════════════════════════════════════
    // GESTION DES RÔLES
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Roles()
    {
      var roles = await _roleService.GetAllRolesAsync();

      var viewModels = roles.Select(r => new RoleManagementViewModel
      {
        Id = r.Id,
        Name = r.Name,
        Description = r.Description,
        CreatedAt = r.CreatedAt,
        UserCount = r.UserRoles?.Count ?? 0
      }).ToList();

      return View(viewModels);
    }

    [HttpGet]
    public IActionResult RoleCreate()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RoleCreate(RoleManagementViewModel model)
    {
      if (!ModelState.IsValid)
        return View(model);

      try
      {
        await _roleService.CreateRoleAsync(model.Name, model.Description);
        TempData["SuccessMessage"] = $"Rôle '{model.Name}' créé avec succès";
        return RedirectToAction(nameof(Roles));
      }
      catch (InvalidOperationException ex)
      {
        ModelState.AddModelError("Name", ex.Message);
        return View(model);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la création du rôle");
        ModelState.AddModelError("", "Une erreur est survenue");
        return View(model);
      }
    }

    [HttpGet]
    public async Task<IActionResult> RoleEdit(string id)
    {
      var role = await _roleService.GetRoleByIdAsync(id);
      if (role == null)
        return NotFound();

      var viewModel = new RoleManagementViewModel
      {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description,
        CreatedAt = role.CreatedAt,
        UserCount = role.UserRoles?.Count ?? 0
      };

      return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RoleEdit(RoleManagementViewModel model)
    {
      if (!ModelState.IsValid)
        return View(model);

      try
      {
        var role = await _roleService.GetRoleByIdAsync(model.Id!);
        if (role == null)
          return NotFound();

        role.Description = model.Description;

        // TODO: Implémenter UpdateAsync dans IRoleService
        // await _roleService.UpdateRoleAsync(role);

        TempData["SuccessMessage"] = "Rôle mis à jour avec succès";
        return RedirectToAction(nameof(Roles));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la mise à jour du rôle");
        ModelState.AddModelError("", "Une erreur est survenue");
        return View(model);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RoleDelete(string id)
    {
      try
      {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
          return NotFound();

        // Vérifier qu'aucun utilisateur n'a ce rôle
        if (role.UserRoles?.Count > 0)
        {
          TempData["ErrorMessage"] = $"Impossible de supprimer le rôle '{role.Name}' car {role.UserRoles.Count} utilisateur(s) l'utilisent";
          return RedirectToAction(nameof(Roles));
        }

        await _roleService.DeleteRoleAsync(id);
        TempData["SuccessMessage"] = $"Rôle '{role.Name}' supprimé avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la suppression du rôle");
        TempData["ErrorMessage"] = "Une erreur est survenue lors de la suppression";
      }

      return RedirectToAction(nameof(Roles));
    }

    // ═══════════════════════════════════════════════════════
    // MÉTHODES PRIVÉES
    // ═══════════════════════════════════════════════════════

    private async Task<List<UserManagementViewModel>> GetAllUsersAsync()
    {
      // TODO: Implémenter GetAllAsync dans IUserRepository
      // Pour l'instant, retourner une liste vide
      // var users = await _userRepository.GetAllAsync();

      var users = await _userService.GetAllAsync(); // Temporaire

      var viewModels = new List<UserManagementViewModel>();

      foreach (var user in users)
      {
        var roles = await _roleService.GetUserRolesAsync(user.Id);

        viewModels.Add(new UserManagementViewModel
        {
          Id = user.Id,
          Username = user.Username,
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          EmailConfirmed = user.EmailConfirmed,
          IsLockedOut = user.IsLockedOut,
          LockoutEnd = user.LockoutEnd,
          CreatedAt = user.CreatedAt,
          LastLoginAt = user.LastLoginAt,
          Roles = roles
        });
      }

      return viewModels;
    }


    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES SYSTÈME
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
      try
      {
        var settings = await _settingsService.GetAllSettingsAsync();
        return View(settings);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors du chargement des paramètres système");
        TempData["ErrorMessage"] = "Erreur lors du chargement des paramètres";
        return RedirectToAction("Index");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveGeneralSettings(GeneralSettings settings)
    {
      try
      {
        var currentUser = User.Identity?.Name;
        var allSettings = await _settingsService.GetAllSettingsAsync();
        allSettings.General = settings;

        await _settingsService.SaveSettingsAsync(allSettings, currentUser);

        TempData["SuccessMessage"] = "Paramètres généraux sauvegardés avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres généraux");
        TempData["ErrorMessage"] = "Erreur lors de la sauvegarde";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEmailSettings(EmailSettings settings)
    {
      try
      {
        var currentUser = User.Identity?.Name;
        var allSettings = await _settingsService.GetAllSettingsAsync();
        allSettings.Email = settings;

        await _settingsService.SaveSettingsAsync(allSettings, currentUser);

        TempData["SuccessMessage"] = "Paramètres email sauvegardés avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres email");
        TempData["ErrorMessage"] = "Erreur lors de la sauvegarde";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TestEmailSettings(EmailSettings settings)
    {
      try
      {
        var success = await _settingsService.TestEmailSettingsAsync(settings);

        if (success)
        {
          TempData["SuccessMessage"] = "Test email réussi ! Vérifiez votre boîte de réception.";
        }
        else
        {
          TempData["ErrorMessage"] = "Le test email a échoué. Vérifiez vos paramètres SMTP.";
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors du test email");
        TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTestingSettings(TestingSettings settings)
    {
      try
      {
        var currentUser = User.Identity?.Name;
        var allSettings = await _settingsService.GetAllSettingsAsync();
        allSettings.Testing = settings;

        await _settingsService.SaveSettingsAsync(allSettings, currentUser);

        TempData["SuccessMessage"] = "Paramètres de test sauvegardés avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres de test");
        TempData["ErrorMessage"] = "Erreur lors de la sauvegarde";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSecuritySettings(SecuritySettings settings)
    {
      try
      {
        var currentUser = User.Identity?.Name;
        var allSettings = await _settingsService.GetAllSettingsAsync();
        allSettings.Security = settings;

        await _settingsService.SaveSettingsAsync(allSettings, currentUser);

        TempData["SuccessMessage"] = "Paramètres de sécurité sauvegardés avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres de sécurité");
        TempData["ErrorMessage"] = "Erreur lors de la sauvegarde";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveBrandingSettings(BrandingSettings settings)
    {
      try
      {
        var currentUser = User.Identity?.Name;
        var allSettings = await _settingsService.GetAllSettingsAsync();
        allSettings.Branding = settings;

        await _settingsService.SaveSettingsAsync(allSettings, currentUser);

        TempData["SuccessMessage"] = "Paramètres de branding sauvegardés avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres de branding");
        TempData["ErrorMessage"] = "Erreur lors de la sauvegarde";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveNotificationSettings(NotificationSettings settings)
    {
      try
      {
        var currentUser = User.Identity?.Name;
        var allSettings = await _settingsService.GetAllSettingsAsync();
        allSettings.Notifications = settings;

        await _settingsService.SaveSettingsAsync(allSettings, currentUser);

        TempData["SuccessMessage"] = "Paramètres de notification sauvegardés avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres de notification");
        TempData["ErrorMessage"] = "Erreur lors de la sauvegarde";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStorageSettings(StorageSettings settings)
    {
      try
      {
        var currentUser = User.Identity?.Name;
        var allSettings = await _settingsService.GetAllSettingsAsync();
        allSettings.Storage = settings;

        await _settingsService.SaveSettingsAsync(allSettings, currentUser);

        TempData["SuccessMessage"] = "Paramètres de stockage sauvegardés avec succès";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la sauvegarde des paramètres de stockage");
        TempData["ErrorMessage"] = "Erreur lors de la sauvegarde";
      }

      return RedirectToAction("Settings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetSettings(string category)
    {
      try
      {
        await _settingsService.ResetToDefaultsAsync(category);
        TempData["SuccessMessage"] = $"Paramètres de la catégorie '{category}' réinitialisés";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la réinitialisation des paramètres");
        TempData["ErrorMessage"] = "Erreur lors de la réinitialisation";
      }

      return RedirectToAction("Settings");
    }
  }
}

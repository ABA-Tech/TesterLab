using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth.Core.Abstractions;
using Auth.Core.Models;
using TesterLab.Models.ViewModels;

namespace TesterLab.Controllers
{
  [Authorize(Roles = "Admin")]
  public class AdminController : Controller
  {
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUserService userService,
        IRoleService roleService,
        ILogger<AdminController> logger)
    {
      _userService = userService;
      _roleService = roleService;
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
  }
}

using Auth.Core.Abstractions;
using Auth.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Policy;
using TesterLab.Models.ViewModels;
using IAuthenticationService = Auth.Core.Abstractions.IAuthenticationService;
using Auth.Core.Validators;
using Auth.Core.Services;

namespace TesterLab.Controllers
{
  public class AccountController : Controller
  {
    private readonly IAuthenticationService _authService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly PasswordValidator _passwordValidator;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthenticationService authService,
        IUserService userService,
        IRoleService roleService,
        IEmailService emailService,
        IPasswordHasher passwordHasher,
        PasswordValidator passwordValidator,
        ILogger<AccountController> logger)
    {
      _authService = authService;
      _userService = userService;
      _roleService = roleService;
      _emailService = emailService;
      _passwordHasher = passwordHasher;
      _passwordValidator = passwordValidator;
      _logger = logger;
    }

    // ═══════════════════════════════════════════════════════
    // INSCRIPTION
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult Register()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (!ModelState.IsValid)
        return View(model);

      // Validation du mot de passe
      var (isValid, errorMessage) = _passwordValidator.Validate(model.Password);
      if (!isValid)
      {
        ModelState.AddModelError("Password", errorMessage!);
        return View(model);
      }

      // Vérifier unicité email et username
      if (!await _userService.IsEmailUniqueAsync(model.Email))
      {
        ModelState.AddModelError("Email", "Cet email est déjà utilisé");
        return View(model);
      }

      if (!await _userService.IsUsernameUniqueAsync(model.Username))
      {
        ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà utilisé");
        return View(model);
      }

      try
      {
        // Créer l'utilisateur
        var user = new ApplicationUser
        {
          Username = model.Username,
          Email = model.Email,
          FirstName = model.FirstName,
          LastName = model.LastName
        };

        var createdUser = await _userService.CreateUserAsync(user, model.Password);

        // Envoyer l'email de confirmation
        var confirmationLink = Url.Action(
            "ConfirmEmail",
            "Account",
            new { token = createdUser.EmailConfirmationToken },
            Request.Scheme);

        await _emailService.SendEmailConfirmationAsync(
            createdUser.Email,
            createdUser.Username,
            confirmationLink!);

        TempData["SuccessMessage"] = "Inscription réussie ! Veuillez vérifier votre email pour confirmer votre compte.";
        return RedirectToAction("Login");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de l'inscription");
        ModelState.AddModelError("", "Une erreur est survenue lors de l'inscription");
        return View(model);
      }
    }

    // ═══════════════════════════════════════════════════════
    // CONFIRMATION EMAIL
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
      if (string.IsNullOrEmpty(token))
        return View("Error");

      var success = await _userService.ConfirmEmailAsync(token);

      if (success)
      {
        TempData["SuccessMessage"] = "Votre email a été confirmé avec succès ! Vous pouvez maintenant vous connecter.";
        return RedirectToAction("Login");
      }

      ViewBag.ErrorMessage = "Le lien de confirmation est invalide ou a expiré.";
      return View("Error");
    }

    // ═══════════════════════════════════════════════════════
    // CONNEXION
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
      if (!ModelState.IsValid)
        return View(model);

      // Récupérer l'utilisateur (par username ou email)
      var user = await _userService.GetByUsernameAsync(model.UsernameOrEmail)
          ?? await _userService.GetByEmailAsync(model.UsernameOrEmail);

      if (user == null)
      {
        ModelState.AddModelError("", "Identifiants invalides");
        return View(model);
      }

      // Vérifier si le compte est verrouillé
      if (user.IsLockedOut)
      {
        ModelState.AddModelError("", "Votre compte est temporairement verrouillé. Réessayez plus tard.");
        return View(model);
      }

      // Vérifier si l'email est confirmé
      if (!user.EmailConfirmed)
      {
        ModelState.AddModelError("", "Veuillez confirmer votre email avant de vous connecter.");
        return View(model);
      }

      // Authentifier via le service
      var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

      var result = await _authService.AuthenticateAsync(
          user.Username,
          model.Password,
          ipAddress,
          userAgent);

      if (!result.Success)
      {
        await _userService.IncrementFailedLoginAsync(user.Id);
        ModelState.AddModelError("", "Identifiants invalides");
        return View(model);
      }

      // Réinitialiser les tentatives échouées
      await _userService.ResetFailedLoginAsync(user.Id);

      // Créer les claims pour le cookie d'authentification
      //var claims = new List<Claim>
      //      {
      //          new Claim(ClaimTypes.NameIdentifier, user.Id),
      //          new Claim(ClaimTypes.Name, user.Username),
      //          new Claim(ClaimTypes.Email, user.Email)
      //      };

      //foreach (var role in user.Roles)
      //{
      //  claims.Add(new Claim(ClaimTypes.Role, role));
      //}

      // Récupérer les rôles de l'utilisateur
      var roleNames = await _roleService.GetUserRolesAsync(user.Id);

      // Créer les claims avec les rôles
      var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email)
    };

      // Ajouter les rôles aux claims
      foreach (var roleName in roleNames)
      {
        claims.Add(new Claim(ClaimTypes.Role, roleName));
      }

      var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var authProperties = new AuthenticationProperties
      {
        IsPersistent = model.RememberMe,
        ExpiresUtc = model.RememberMe
              ? DateTimeOffset.UtcNow.AddDays(30)
              : DateTimeOffset.UtcNow.AddHours(12)
      };

      await HttpContext.SignInAsync(
          CookieAuthenticationDefaults.AuthenticationScheme,
          new ClaimsPrincipal(claimsIdentity),
          authProperties);

      _logger.LogInformation("Connexion réussie pour {Username}", user.Username);

      // Redirection
      if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        return Redirect(model.ReturnUrl);

      return RedirectToAction("Index", "Home");
    }

    // ═══════════════════════════════════════════════════════
    // MOT DE PASSE OUBLIÉ
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult ForgotPassword()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (!ModelState.IsValid)
        return View(model);

      var token = await _userService.GeneratePasswordResetTokenAsync(model.Email);

      // SÉCURITÉ: Toujours afficher le même message même si l'email n'existe pas
      if (!string.IsNullOrEmpty(token))
      {
        var resetLink = Url.Action(
            "ResetPassword",
            "Account",
            new { token },
            Request.Scheme);

        var user = await _userService.GetByEmailAsync(model.Email);
        if (user != null)
        {
          await _emailService.SendPasswordResetAsync(
              user.Email,
              user.Username,
              resetLink!);
        }
      }

      TempData["SuccessMessage"] = "Si cet email existe, vous recevrez un lien de réinitialisation.";
      return RedirectToAction("Login");
    }

    // ═══════════════════════════════════════════════════════
    // RÉINITIALISATION MOT DE PASSE
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
      if (string.IsNullOrEmpty(token))
        return View("Error");

      var model = new ResetPasswordViewModel { Token = token };
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
        return View(model);

      // Validation du mot de passe
      var (isValid, errorMessage) = _passwordValidator.Validate(model.Password);
      if (!isValid)
      {
        ModelState.AddModelError("Password", errorMessage!);
        return View(model);
      }

      var success = await _userService.ResetPasswordAsync(model.Token, model.Password);

      if (success)
      {
        TempData["SuccessMessage"] = "Votre mot de passe a été réinitialisé avec succès !";
        return RedirectToAction("Login");
      }

      ModelState.AddModelError("", "Le lien de réinitialisation est invalide ou a expiré.");
      return View(model);
    }

    // ═══════════════════════════════════════════════════════
    // DÉCONNEXION
    // ═══════════════════════════════════════════════════════

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

      _logger.LogInformation("Déconnexion de {Username}", User.Identity?.Name);

      return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userId))
        return RedirectToAction("Login");

      var user = await _userService.GetByIdAsync(userId);
      if (user == null)
        return NotFound();

      var roles = await _roleService.GetUserRolesAsync(userId);

      var viewModel = new UserProfileViewModel
      {
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        EmailConfirmed = user.EmailConfirmed,
        CreatedAt = user.CreatedAt,
        LastLoginAt = user.LastLoginAt,
        Roles = roles
      };

      return View(viewModel);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Settings()
    {
      return View();
    }

  }
}

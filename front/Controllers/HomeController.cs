using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using TesterLab.Domain.interfaces.Services;

namespace AspnetCoreMvcFull.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IApplicationService _applicationService;

    public HomeController(ILogger<HomeController> logger, IApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    public async Task<IActionResult> Index()
    {
        var applications = await _applicationService.GetAllApplicationsAsync();
        ViewBag.Applications = applications;
        return View(applications);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

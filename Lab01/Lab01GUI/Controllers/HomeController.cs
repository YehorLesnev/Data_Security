using Lab01GUI.Pages.Home;
using Lab01GUI.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Lab01GUI.Controllers;

public class HomeController(ILogger<ErrorModel> logger) : Controller
{
	public IActionResult Index()
	{
        ViewData["Title"] = "Home page";
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorModel(logger));
	}
}

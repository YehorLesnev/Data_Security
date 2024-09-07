using System.Text;
using Lab01GUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lab01GUI.Controllers;

[Route("[Controller]/[Action]")]
public class RandomNumberController(IRandomNumberGeneratorService randomNumberService) : Controller
{
	[Route("[Action]")]
	public IActionResult Index()
	{
		return View("Index");
	}

	[HttpPost]
	public IActionResult Generate(uint x0, uint m, uint a, uint c)
	{
		var randomNumbers = randomNumberService.GetRandomNumbers(x0, m, a, c).ToList();
		ViewBag.RandomNumbers = randomNumbers;

		return View("Result");
	}

	[HttpPost]
	public IActionResult Download(uint x0, uint m, uint a, uint c)
	{
		var randomNumbers = randomNumberService.GetRandomNumbers(x0, m, a, c);
		var content = string.Join(Environment.NewLine, randomNumbers);

		byte[] bytes = Encoding.UTF8.GetBytes(content);
		return File(bytes, "text/plain", "RandomNumbers.txt");
	}
}
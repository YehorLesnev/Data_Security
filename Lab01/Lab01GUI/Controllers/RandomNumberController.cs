using Lab01GUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lab01GUI.Controllers;

[Route("[Controller]/[Action]")]
public class RandomNumberController(
	IRandomNumberGeneratorService randomNumberService,
	IFileWriterService fileWriterService) : Controller
{
	[Route("[Action]")]
	public IActionResult Index()
	{
		return View("Index");
	}

	[HttpPost]
	public IActionResult Generate(uint x0, uint m, uint a, uint c, uint numOfNumbers, int pageNumber = 1, int pageSize = 1000)
	{
		var randomNumbers = randomNumberService.GetRandomNumbers(x0, m, a, c, numOfNumbers).ToList();

		while ((randomNumbers.Count - (pageSize * pageNumber)) <= -pageSize)
		{
			--pageNumber;
		}

		var paginatedNumbers = randomNumbers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

		ViewBag.TotalPages = (int)Math.Ceiling(randomNumbers.Count / (double)pageSize);
		ViewBag.PageNumber = pageNumber;
		ViewBag.PageSize = pageSize;
		ViewBag.RandomNumbers = paginatedNumbers;

		return View("Result");
	}

	[HttpPost]
	public IActionResult Download(uint x0, uint m, uint a, uint c, uint numOfNumbers)
	{
		var randomNumbers = randomNumberService.GetRandomNumbers(x0, m, a, c, numOfNumbers);
		
		return File(fileWriterService.WriteToFile(randomNumbers, "n\t\tXn"), "text/plain", "RandomNumbers.txt");
	}
}
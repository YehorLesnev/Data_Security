using Lab01GUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lab01GUI.Controllers;

[RequestSizeLimit(1_000_000_000)]
[RequestFormLimits(MultipartBodyLengthLimit = 1_000_000_000)]
public class MD5Controller(
	IMD5Service mD5Service,
	IFileWriterService fileWriterService) : Controller
{
	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}

	[HttpGet]
	public IActionResult IntegrityCheck()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> IntegrityCheck(IFormFile file, string md5hash)
	{
		if (file is null) throw new ArgumentException("File is not selected or too big");

		ViewBag.Hash = md5hash;
		ViewBag.Result = false;
		
		using (var memoryStream = new MemoryStream())
		{
			await file.CopyToAsync(memoryStream);

			string hash = mD5Service.GetHash(memoryStream.ToArray()).ToUpper();

			ViewBag.Result = hash.Equals(md5hash, StringComparison.InvariantCultureIgnoreCase);
		}

		return View();
	}

	[HttpPost]
	public IActionResult GetHash(string inputText)
	{
		if (string.IsNullOrEmpty(inputText)) inputText = "";

		string hash = mD5Service.GetHash(inputText).ToUpper();

		ViewBag.Hash = hash;
		ViewBag.InputText = inputText;
		return View("Index");
	}

	[HttpPost]
	public async Task<IActionResult> ComputeHashFromFile(IFormFile uploadedFile)
	{
		if (uploadedFile is null) throw new ArgumentException("File is not selected or too big");

		ViewBag.FileName = uploadedFile.FileName;

		using (var memoryStream = new MemoryStream())
		{
			await uploadedFile.CopyToAsync(memoryStream);

			string hash = mD5Service.GetHash(memoryStream.ToArray()).ToUpper();

			ViewBag.Hash = hash;
		}

		return View("Index");
	}

	[HttpPost]
	public IActionResult Download(string hashValue)
	{
		return File(fileWriterService.WriteToFile(hashValue), "text/plain", "Hash.txt");
	}
}

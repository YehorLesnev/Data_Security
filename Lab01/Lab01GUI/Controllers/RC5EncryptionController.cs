using Lab01GUI.Services.Implementation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Lab01GUI.Controllers;

[RequestSizeLimit(2_000_000_000)]
[RequestFormLimits(MultipartBodyLengthLimit = 2_000_000_000)]
public class RC5EncryptionController : Controller
{
	private readonly RC5_CBC_PadService _rc5Service = new(
		new RC5_CBC_PadService.WordLength(16), // WordLength
		12, // Number of Rounds
		16 // Secret Key Length in Bytes
		);

	[HttpGet]
	public IActionResult Encrypt()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Upload(IFormFile file, string password)
	{
		if (file == null || file.Length == 0)
		{
			return BadRequest("No file uploaded.");
		}

		using var memoryStream = new MemoryStream();

		await file.CopyToAsync(memoryStream);
		var fileBytes = memoryStream.ToArray();

		var encryptedFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".enc";

		Stopwatch stopwatch = new();
		stopwatch.Start();

		var encryptedBytes = _rc5Service.Encrypt(fileBytes, password);

		string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.enc");
		await System.IO.File.WriteAllBytesAsync(tempFilePath, encryptedBytes);

		stopwatch.Stop();
		TempData["EncryptedFilePath"] = tempFilePath;
		TempData["EncryptedFileName"] = Path.GetFileNameWithoutExtension(file.FileName) + ".enc";
		TempData["ExecutionTime"] = (int)stopwatch.ElapsedMilliseconds;
		TempData["InputLength"] = (int)file.Length;

		return RedirectToAction("ShowEncryptionResult");
	}

	[HttpGet]
	public IActionResult Decrypt()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Decrypt(IFormFile file, string password)
	{
		if (file == null || file.Length == 0)
		{
			return BadRequest("No file uploaded.");
		}

		using var memoryStream = new MemoryStream();

		await file.CopyToAsync(memoryStream);
		var fileBytes = memoryStream.ToArray();

		Stopwatch stopwatch = new();
		stopwatch.Start();

		var decryptedBytes = _rc5Service.Decrypt(fileBytes, password);

		stopwatch.Stop();

		string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dec");
		await System.IO.File.WriteAllBytesAsync(tempFilePath, decryptedBytes);

		TempData["DecryptedFilePath"] = tempFilePath;
		TempData["DecryptedFileName"] = Path.GetFileNameWithoutExtension(file.FileName) + ".dec";
		TempData["ExecutionTime"] = (int)stopwatch.ElapsedMilliseconds;
		TempData["InputLength"] = (int)file.Length;

		return RedirectToAction("ShowDecryptionResult");
	}

	[HttpGet]
	public IActionResult ShowDecryptionResult()
	{
		// Retrieve decryption time and input length from TempData
		ViewBag.ExecutionTime = TempData["ExecutionTime"];
		ViewBag.InputLength = TempData["InputLength"];

		return View("Decrypt");
	}

	
	[HttpGet]
	public IActionResult ShowEncryptionResult()
	{
		// Retrieve decryption time and input length from TempData
		ViewBag.ExecutionTime = TempData["ExecutionTime"];
		ViewBag.InputLength = TempData["InputLength"];

		return View("Encrypt");
	}

	[HttpGet]
	public IActionResult DownloadEncryptedFile()
	{
		if (TempData["EncryptedFilePath"] is not string filePath || !System.IO.File.Exists(filePath))
		{
			return NoContent(); // No file available for download
		}

		var fileName = TempData["EncryptedFileName"] as string ?? "encrypted-file.enc";

		// Read the file from disk and return it as a download
		var fileBytes = System.IO.File.ReadAllBytes(filePath);

		// Delete the file after it's downloaded
		System.IO.File.Delete(filePath);

		return File(fileBytes, "application/octet-stream", fileName);
	}

	[HttpGet]
	public IActionResult DownloadDecryptedFile()
	{
		if (TempData["DecryptedFilePath"] is not string filePath || !System.IO.File.Exists(filePath))
		{
			return NoContent(); // No file available for download
		}

		var fileName = TempData["DecryptedFileName"] as string ?? "decrypted-file.dec";

		// Read the file from disk and return it as a download
		var fileBytes = System.IO.File.ReadAllBytes(filePath);

		// Delete the file after it's downloaded
		System.IO.File.Delete(filePath);

		return File(fileBytes, "application/octet-stream", fileName);
	}
}

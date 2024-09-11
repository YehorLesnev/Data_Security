using Lab01GUI.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Lab01GUI.Controllers;

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

        var encryptedBytes = _rc5Service.Encrypt(fileBytes, password);
        var encryptedFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".enc";

        return File(encryptedBytes, "application/octet-stream", encryptedFileName);
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

        var decryptedBytes = _rc5Service.Decrypt(fileBytes, password);
        var decryptedFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".dec";

        return File(decryptedBytes, "application/octet-stream", decryptedFileName);
    }
}

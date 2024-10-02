using Lab01GUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lab01GUI.Controllers;

[RequestSizeLimit(2_000_000_000)]
[RequestFormLimits(MultipartBodyLengthLimit = 2_000_000_000)]
public class RSAEncryptionController(IRSAEncryptionService rsaService) : Controller
{
    [HttpGet]
    public IActionResult GenerateKeys()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GenerateKeysAction(int keySize)
    {
        rsaService.SetKeySize(keySize);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var keyResult = rsaService.GenerateKeys();

        stopwatch.Stop();
        ViewBag.ExecutionTime = stopwatch.ElapsedMilliseconds;

        ViewBag.PublicKey = keyResult.PublicKey;
        ViewBag.PrivateKey = keyResult.PrivateKey;

        return View("GenerateKeysResult");
    }

    [HttpGet]
    public IActionResult Encrypt()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EncryptAction(string plainText, IFormFile publicKeyFile)
    {
        if (string.IsNullOrWhiteSpace(plainText) || publicKeyFile == null)
        {
            return BadRequest("File to encrypt and public key file are required.");
        }

        using var publicKeyStream = new StreamReader(publicKeyFile.OpenReadStream());
        var publicKey = await publicKeyStream.ReadToEndAsync();

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var encryptedBytes = rsaService.Encrypt(plainText, publicKey);

        stopwatch.Stop();
        ViewBag.ExecutionTime = stopwatch.ElapsedMilliseconds;
        ViewBag.InputLength = plainText.Length;

        TempData["EncryptedFile"] = Convert.ToBase64String(encryptedBytes);

        return View("EncryptResult");
    }

    [HttpGet]
    public IActionResult DownloadEncryptedFile()
    {
		if (TempData["EncryptedFile"] is not string encryptedFileBase64)
		{
			return NotFound("No encrypted file available.");
		}

		var encryptedFileBytes = Convert.FromBase64String(encryptedFileBase64);

        return File(encryptedFileBytes, "application/octet-stream", "rsa-encrypted.enc");
    }

    [HttpGet]
    public IActionResult Decrypt()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DecryptAction(IFormFile file, IFormFile privateKeyFile)
    {
        if (file == null || privateKeyFile == null)
        {
            return BadRequest("Encrypted file, key file, and private key file are required.");
        }

        using var fileMemoryStream = new MemoryStream();
        await file.CopyToAsync(fileMemoryStream);
        var encryptedFileBytes = fileMemoryStream.ToArray();

        using var privateKeyStream = new StreamReader(privateKeyFile.OpenReadStream());
        var privateKey = await privateKeyStream.ReadToEndAsync();

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var decryptedText = rsaService.Decrypt(encryptedFileBytes, privateKey);

        stopwatch.Stop();
        ViewBag.ExecutionTime = stopwatch.ElapsedMilliseconds;
        ViewBag.InputLength = decryptedText.Length;

        ViewBag.DecryptedText = decryptedText;

        return View("DecryptResult");
    }
}
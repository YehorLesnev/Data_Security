using Lab01GUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

	// POST: RSAEncryption/GenerateKeys
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult GenerateKeysAction(int keySize)
	{
		rsaService.SetKeySize(keySize);
		var keyResult = rsaService.GenerateKeys();

		ViewBag.PublicKey = keyResult.PublicKey;
		ViewBag.PrivateKey = keyResult.PrivateKey;

		return View("GenerateKeysResult");
	}

	// Download the public key as a file
	[HttpGet]
	public IActionResult DownloadPublicKey()
	{
		var keyResult = rsaService.GenerateKeys();
		var publicKeyBytes = Encoding.UTF8.GetBytes(keyResult.PublicKey);
		return File(publicKeyBytes, "application/xml", "publicKey.xml");
	}

	// Download the private key as a file
	[HttpGet]
	public IActionResult DownloadPrivateKey()
	{
		var keyResult = rsaService.GenerateKeys();
		var privateKeyBytes = Encoding.UTF8.GetBytes(keyResult.PrivateKey);
		return File(privateKeyBytes, "application/xml", "privateKey.xml");
	}

	// GET: RSAEncryption/Encrypt
	[HttpGet]
	public IActionResult Encrypt()
	{
		return View();
	}

	// POST: RSAEncryption/Encrypt
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EncryptAction(string plainText, IFormFile publicKeyFile)
	{
		if (string.IsNullOrWhiteSpace(plainText) || publicKeyFile == null)
		{
			return BadRequest("File to encrypt and public key file are required.");
		}

		// Read the public key file
		using var publicKeyStream = new StreamReader(publicKeyFile.OpenReadStream());
		var publicKey = await publicKeyStream.ReadToEndAsync();

		// Encrypt the data
		var encryptedBytes = rsaService.Encrypt(plainText, publicKey);

		return File(encryptedBytes, "application/octet-stream", "rsa-encrypted.enc");
	}

	// GET: RSAEncryption/Decrypt
	[HttpGet]
	public IActionResult Decrypt()
	{
		return View();
	}

	// POST: RSAEncryption/Decrypt
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DecryptAction(IFormFile file, IFormFile privateKeyFile)
	{
		if (file == null || privateKeyFile == null)
		{
			return BadRequest("Encrypted file, key file, and private key file are required.");
		}

		// Read the encrypted file
		using var fileMemoryStream = new MemoryStream();
		await file.CopyToAsync(fileMemoryStream);
		var encryptedFileBytes = fileMemoryStream.ToArray();

		// Read the private key file
		using var privateKeyStream = new StreamReader(privateKeyFile.OpenReadStream());
		var privateKey = await privateKeyStream.ReadToEndAsync();
		
		// Decrypt the data
		var decryptedText = rsaService.Decrypt(encryptedFileBytes, privateKey);

		ViewBag.DecryptedText = decryptedText;

		return View("DecryptResult");
	}
}
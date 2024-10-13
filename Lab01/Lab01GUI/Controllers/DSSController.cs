using Lab01GUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text;

namespace Lab01GUI.Controllers;

[RequestSizeLimit(2_000_000_000)]
[RequestFormLimits(MultipartBodyLengthLimit = 2_000_000_000)]
public class DSSController(IDSSService dSSService) : Controller
{
	[HttpGet]
	public IActionResult GenerateKeys()
	{
		return View("GenerateKeys");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult GenerateKeysAction(int keySize)
	{
		dSSService.SetKeySize(keySize);
		var keyResult = dSSService.GetKeyResult();

		ViewBag.PublicKey = keyResult.PublicKey;
		ViewBag.PrivateKey = keyResult.PrivateKey;

		return View("GenerateKeysResult");
	}

	[HttpGet]
	public IActionResult Sign()
	{
		return View();
	}

	[HttpGet]
	public IActionResult Verify()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> SignText(IFormFile privateKeyFile, string inputText)
	{
		if (privateKeyFile != null && !string.IsNullOrEmpty(inputText))
		{
			byte[] signature = await dSSService.Sign(inputText, privateKeyFile);
			string signatureHex = BitConverter.ToString(signature).Replace("-", "");
			ViewBag.Signature = signatureHex;
			ViewBag.SignatureBytes = Convert.ToBase64String(signature);

			return View("Sign");
		}
		return BadRequest("Please upload a valid private key file and provide data to sign.");
	}

	[HttpPost]
	public async Task<IActionResult> VerifyText(string inputText, string SignatureText, IFormFile SignatureFile, IFormFile publicKeyFile)
	{
		if (!string.IsNullOrEmpty(inputText))
		{
			if(SignatureFile is not null)
			{
				ViewBag.IsValid = await dSSService.Verify(inputText, SignatureFile, publicKeyFile);
			}
			else if(!string.IsNullOrWhiteSpace(SignatureText))
			{
				ViewBag.IsValid = await dSSService.Verify(inputText, SignatureText, publicKeyFile);
			}

			ViewBag.Message = ViewBag.IsValid ? "Signature is valid" : "Signature is invalid";

			return View("Verify");
		}

		return BadRequest("Please upload a valid public key file and signature.");
	}

	[HttpPost]
	public async Task<IActionResult> SignFile(IFormFile inputFile, IFormFile privateKeyFile)
	{
		if (inputFile is not null && inputFile.Length > 0)
		{
			var signature = await dSSService.Sign(inputFile, privateKeyFile);
			string signatureHex = BitConverter.ToString(signature).Replace("-", "");
			ViewBag.Signature = signatureHex;
			ViewBag.SignatureBytes = Convert.ToBase64String(signature);
		}

		return View("Sign");
	}

	[HttpPost]
	public async Task<IActionResult> VerifyFile(IFormFile inputFile, IFormFile SignatureFile, IFormFile publicKeyFile)
	{
		if (inputFile is not null && SignatureFile is not null)
		{
			ViewBag.IsValid = await dSSService.Verify(inputFile, SignatureFile, publicKeyFile);
			ViewBag.Message = ViewBag.IsValid ? "The file signature is valid." : "The file signature is not valid.";
		}
		else
		{
			ViewBag.Message = "Please upload both a file and a signature file.";
		}

		return View("Verify");
	}

	[HttpGet]
	public IActionResult DownloadSignature(string signature)
	{
		if (string.IsNullOrWhiteSpace(signature))
			return BadRequest("Signature is empty or invalid.");

		byte[] bytes = Convert.FromBase64String(signature);

		if (bytes is not null && bytes.Length > 0)
		{
			var fileName = "signature.sign";
			return File(bytes, "application/octet-stream", fileName);
		}

		return BadRequest("Signature is empty or invalid.");
	}
}
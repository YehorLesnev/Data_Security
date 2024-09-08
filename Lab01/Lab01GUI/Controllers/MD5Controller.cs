using System.Security.Cryptography;
using Lab01GUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lab01GUI.Controllers;

[RequestSizeLimit(1_000_000_000)]
[RequestFormLimits(MultipartBodyLengthLimit = 1_000_000_000)]
public class MD5Controller(IMD5Service mD5Service) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult GetHash(string inputText)
    {
        if(string.IsNullOrEmpty(inputText)) inputText = "";

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
}

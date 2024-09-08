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
        string hash = mD5Service.GetHash(inputText);

        ViewBag.Hash = hash;
        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ComputeHashFromFile(IFormFile uploadedFile)
    {
        if (uploadedFile is null) return View("Index");

        using (var memoryStream = new MemoryStream())
        {
            await uploadedFile.CopyToAsync(memoryStream);

            string hash = mD5Service.GetHash(memoryStream.ToArray());

            ViewBag.Hash = hash;
        }

        return View("Index");
    }
}

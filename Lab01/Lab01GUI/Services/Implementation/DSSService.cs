using Lab01GUI.Services.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Lab01GUI.Services.Implementation;

public class DSSService : IDSSService
{
	private int KeySize = 2048;

	public void SetKeySize(int keySize)
	 => KeySize = keySize;

	public int GetKeySize()
		=> KeySize;

	public DSSKeyResult GetKeyResult()
	{
		using var dsa = new DSACryptoServiceProvider(KeySize);

		var privateKey = dsa.ToXmlString(true);
		var publicKey = dsa.ToXmlString(false);

		return new(publicKey, privateKey);
	}

	public byte[] Sign(string data, string privateKey)
	{
		return Sign(Encoding.UTF8.GetBytes(data), privateKey);
	}

	public async Task<byte[]> Sign(IFormFile file, string privateKey)
	{
		return Sign(await GetFileBytesAsync(file), privateKey);
	}

	public byte[] Sign(byte[] data, string privateKey)
	{
		using var dsa = new DSACryptoServiceProvider();
		privateKey = WebUtility.HtmlDecode(privateKey);
		dsa.FromXmlString(privateKey);

		return dsa.SignData(data);
	}

	public async Task<bool> Verify(string data, IFormFile SignatureFile, string publicKey)
	{
		var signatureBytes = await GetFileBytesAsync(SignatureFile);

		return Verify(Encoding.UTF8.GetBytes(data), signatureBytes, publicKey);
	}

	public async Task<bool> Verify(IFormFile file, IFormFile SignatureFile, string publicKey)
	{
		publicKey = WebUtility.HtmlDecode(publicKey);

		var fileData = await GetFileBytesAsync(file);
		var signatureBytes = await GetFileBytesAsync(SignatureFile);

		return Verify(fileData, signatureBytes, publicKey);
	}

	public bool Verify(byte[] data, byte[] signature, string publicKey)
	{
		publicKey = WebUtility.HtmlDecode(publicKey);

		using var dsa = new DSACryptoServiceProvider();
		dsa.FromXmlString(publicKey);

		return dsa.VerifyData(data, signature);
	}

	public async Task<byte[]> Sign(string data, IFormFile privateKey)
	{
		return Sign(data, await GetFileTextAsync(privateKey));
	}

	public async Task<byte[]> Sign(IFormFile file, IFormFile privateKey)
	{
		return await Sign(file, await GetFileTextAsync(privateKey));
	}

	public async Task<bool> Verify(string data, IFormFile SignatureFile, IFormFile publicKey)
	{
		return await Verify(data, SignatureFile, await GetFileTextAsync(publicKey));
	}

	public async Task<bool> Verify(IFormFile file, IFormFile SignatureFile, IFormFile publicKey)
	{
		return await Verify(file, SignatureFile, await GetFileTextAsync(publicKey));
	}

	public async Task<bool> Verify(string data, string SignatureText, IFormFile publicKey)
	{
		return Verify(Encoding.UTF8.GetBytes(data), Convert.FromHexString(SignatureText), await GetFileTextAsync(publicKey));
	}

	public async Task<bool> Verify(IFormFile file, string SignatureText, IFormFile publicKey)
	{
		return Verify(await GetFileBytesAsync(file), Convert.FromHexString(SignatureText), await GetFileTextAsync(publicKey));
	}

	private static async Task<byte[]> GetFileBytesAsync(IFormFile file)
	{
		using var stream = file.OpenReadStream();
		using var memoryStream = new MemoryStream();
		await stream.CopyToAsync(memoryStream);
		return memoryStream.ToArray();
	}

	private static async Task<string> GetFileTextAsync(IFormFile file)
	{
		using var stream = file.OpenReadStream();
		using var reader = new StreamReader(stream);
		return WebUtility.HtmlDecode(await reader.ReadToEndAsync());
	}
}

public record DSSKeyResult(string PublicKey, string PrivateKey);
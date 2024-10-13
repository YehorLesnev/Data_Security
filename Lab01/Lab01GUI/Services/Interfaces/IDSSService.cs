using Lab01GUI.Services.Implementation;

namespace Lab01GUI.Services.Interfaces;

public interface IDSSService
{
	DSSKeyResult GetKeyResult();

	byte[] Sign(string data, string privateKey);

	Task<byte[]> Sign(string data, IFormFile privateKey);

	Task<byte[]> Sign(IFormFile file, string privateKey);

	Task<byte[]> Sign(IFormFile file, IFormFile privateKey);

	byte[] Sign(byte[] data, string privateKey);

	Task<bool> Verify(string data, IFormFile SignatureFile, string publicKey);

	Task<bool> Verify(string data, IFormFile SignatureFile, IFormFile publicKey);

	bool Verify(byte[] data, byte[] signature, string publicKey);

	Task<bool> Verify(IFormFile file, IFormFile SignatureFile, string publicKey);

	Task<bool> Verify(IFormFile file, IFormFile SignatureFile, IFormFile publicKey);

	void SetKeySize(int keySize);

	int GetKeySize();
}

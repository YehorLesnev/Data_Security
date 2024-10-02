using Lab01GUI.Services.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Lab01GUI.Services.Implementation;

public class RSAEncryptionService : IRSAEncryptionService
{
    private int KeySize = 2048;

	public void SetKeySize(int keySize) 
	 => KeySize = keySize; 

	public int GetKeySize()
		=> KeySize;

    public RSAKeyResult GenerateKeys()
	{
		using var rsa = new RSACryptoServiceProvider(KeySize);

		try
		{
			string publicKey = rsa.ToXmlString(false); // false means public key only
			string privateKey = rsa.ToXmlString(true); // true means public and private key

			return new RSAKeyResult(publicKey, privateKey);
		}
		finally
		{
			rsa.PersistKeyInCsp = false; // Avoid saving the keys in a key container
		}
	}

	public byte[] Encrypt(string plainText, string publicKey)
	{
		using var rsa = new RSACryptoServiceProvider();

		try
		{
			publicKey = WebUtility.HtmlDecode(publicKey);
			rsa.FromXmlString(publicKey);
			var byteData = Encoding.UTF8.GetBytes(plainText);
			var result = rsa.Encrypt(byteData, false);
			return result; // OAEP (Optimal asymmetric encryption padding) padding - will use PKCS (Public Key Cryptography Standards) #1 v1.5 padding if false
		}
		finally
		{
			rsa.PersistKeyInCsp = false;
		}
	}

	public string Decrypt(string cipherText, string privateKey)
		=> Decrypt(Encoding.UTF8.GetBytes(cipherText), privateKey);

	public string Decrypt(byte[] cipherText, string privateKey)
	{
		using var rsa = new RSACryptoServiceProvider();

		try
		{
			privateKey = WebUtility.HtmlDecode(privateKey);
			rsa.FromXmlString(privateKey);
			var decryptedBytes = rsa.Decrypt(cipherText, false); // false for OAEP padding

			return Encoding.UTF8.GetString(decryptedBytes);
		}
		finally
		{
			rsa.PersistKeyInCsp = false;
		}
	}
}

public record RSAKeyResult(string PublicKey, string PrivateKey);
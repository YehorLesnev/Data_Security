using Lab01GUI.Services.Implementation;

namespace Lab01GUI.Services.Interfaces;

public interface IRSAEncryptionService
{
    RSAKeyResult GenerateKeys();
    byte[] Encrypt(string plainText, string publicKey);

    string Decrypt(string cipherText, string privateKey);

    string Decrypt(byte[] cipherText, string privateKey);

    void SetKeySize(int keySize);

    int GetKeySize();
}

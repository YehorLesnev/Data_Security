namespace Lab01GUI.Services.Interfaces;

public interface IMD5Service
{
    string GetHash(string input);


    string GetHash(byte[] input);
}

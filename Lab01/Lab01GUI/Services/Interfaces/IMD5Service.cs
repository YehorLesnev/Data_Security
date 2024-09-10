namespace Lab01GUI.Services.Interfaces;

public interface IMD5Service
{
	byte[] GetHash(string input);


    byte[] GetHash(byte[] input);


    string GetHashString(string input);

    string GetHashString(byte[] input);
}

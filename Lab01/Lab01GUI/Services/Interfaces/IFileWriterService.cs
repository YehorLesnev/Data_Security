namespace Lab01GUI.Services.Interfaces;

public interface IFileWriterService
{
	public byte[] WriteToFile(IEnumerable<uint> randomNumbers, string header);
}

using Lab01GUI.Services.Interfaces;
using System.Text;

namespace Lab01GUI.Services.Implementation;

public class FileWriterService : IFileWriterService
{
	public byte[] WriteToFile(IEnumerable<uint> randomNumbers, string header)
	{
		var content = new StringBuilder();
		content.AppendLine(header);

		int index = 1;

		foreach (var number in randomNumbers)
		{
			content.AppendLine($"{index}\t\t{number}");
			++index;
		}

		return Encoding.UTF8.GetBytes(content.ToString());
	}
}

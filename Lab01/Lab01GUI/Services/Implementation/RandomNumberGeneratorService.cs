using Lab01GUI.Services.Interfaces;

namespace Lab01GUI.Services.Implementation;

public class RandomNumberGeneratorService : IRandomNumberGeneratorService
{
	public IEnumerable<uint> GetRandomNumbers(uint x0, uint m, uint a, uint c)
	{
		var xn = x0;

		for (uint i = 0; i < m; ++i)
		{
			xn = (a * xn + c) % m;

			yield return xn;
		}
	}
}

using Lab01GUI.Services.Interfaces;

namespace Lab01GUI.Services.Implementation;

public class RandomNumberGeneratorService : IRandomNumberGeneratorService
{
	public IEnumerable<uint> GetRandomNumbers(uint x0, uint m, uint a, uint c, uint numOfNumbers)
	{
		if(numOfNumbers >= int.MaxValue)
		{
			throw new ArgumentException("Number of numbers cannot be int.MaxValue or more");
		}

		var xn = x0;

		for (uint i = 0; i < numOfNumbers; ++i)
		{
			xn = (a * xn + c) % m;

			yield return xn;
		}
	}
}

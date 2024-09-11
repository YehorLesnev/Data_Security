using Lab01GUI.Services.Interfaces;

namespace Lab01GUI.Services.Implementation;

public class RandomNumberGeneratorService : IRandomNumberGeneratorService
{
	private uint _seed = (uint)DateTime.Now.Ticks;

	public IEnumerable<uint> GetRandomNumbers(uint x0, uint m, uint a, uint c, uint numOfNumbers)
	{
		if (numOfNumbers >= int.MaxValue)
		{
			throw new ArgumentException("Number of numbers cannot be int.MaxValue or more");
		}

		_seed = x0;

		for (uint i = 0; i < numOfNumbers; ++i)
		{
			_seed = (a * _seed + c) % m;

			yield return _seed;
		}
	}

	public uint Next()
	{
		uint a = 16807;
		uint c = 0;
		uint m = int.MaxValue - 1;
		var x0 = _seed % m;

		return GetRandomNumbers(x0, m, a, c, 1).FirstOrDefault();
	}
}

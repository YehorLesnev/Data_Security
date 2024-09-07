namespace Lab01GUI.Services.Interfaces;

public interface IRandomNumberGeneratorService
{
	IEnumerable<uint> GetRandomNumbers(uint x0, uint m, uint a, uint c);
}

using Lab01.Services.Implementation;

var randomNumberService = new RandomNumberGeneratorService();

uint x0 = 13u;
uint m = 10000u;
uint a = 13 * 13 * 13;
uint c = 1597u;

foreach (var x in randomNumberService.GetRandomNumbers(x0, m, a, c))
{
	Console.WriteLine(x);
}
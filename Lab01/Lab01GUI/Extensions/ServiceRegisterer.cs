using Lab01GUI.Services.Implementation;
using Lab01GUI.Services.Interfaces;

namespace Lab01GUI.Extensions;

public static class ServiceRegisterer
{
	public static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		services.AddScoped<IRandomNumberGeneratorService, RandomNumberGeneratorService>();
		services.AddScoped<IFileWriterService, FileWriterService>();
		services.AddScoped<IMD5Service, MD5Service>();
		services.AddScoped<IRSAEncryptionService, RSAEncryptionService>();
		services.AddScoped<IDSSService, DSSService>();

		return services;
	}
}

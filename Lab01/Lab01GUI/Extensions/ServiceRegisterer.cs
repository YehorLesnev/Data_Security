﻿using Lab01GUI.Services.Implementation;
using Lab01GUI.Services.Interfaces;

namespace Lab01GUI.Extensions;

public static class ServiceRegisterer
{
	public static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		services.AddScoped<IRandomNumberGeneratorService, RandomNumberGeneratorService>();

		return services;
	}
}

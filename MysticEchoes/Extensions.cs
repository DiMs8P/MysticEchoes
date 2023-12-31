﻿using Microsoft.Extensions.DependencyInjection;
using MysticEchoes.Core;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Json;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Implementations;

namespace MysticEchoes;

public static class Extensions
{
    public static void AddGame(this IServiceCollection services)
    {
        services.AddTransient<IMazeGenerator, MazeGeneratorAdapter>();
        services.AddTransient<IInputManager, BaseInputManager>();
        services.AddTransient<IDataLoader, JsonLoader>();

        services.AddScoped<SystemExecutionContext>();
        services.AddScoped<PrefabManager>();
        services.AddScoped<AssetManager>();
        services.AddScoped<AnimationManager>();
        services.AddScoped<Game>();
    }
}

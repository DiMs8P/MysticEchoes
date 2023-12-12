using Microsoft.Extensions.DependencyInjection;
using MysticEchoes.Core;
using MysticEchoes.Core.Assets;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Implementation;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Implementations;

namespace MysticEchoes;

public static class Extensions
{
    public static void AddGame(this IServiceCollection services)
    {
        services.AddTransient<IMazeGenerator, MazeGeneratorAdapter>();
        services.AddTransient<IInputManager, BaseInputManager>();
        services.AddTransient<ILoader, JsonLoader>();

        services.AddScoped<AssetManager>();
        services.AddScoped<Game>();
    }
}

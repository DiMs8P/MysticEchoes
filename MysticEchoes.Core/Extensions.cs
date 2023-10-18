using Microsoft.Extensions.DependencyInjection;
using MysticEchoes.Core.Base;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Rendering;

namespace MysticEchoes.Core;

public static class Extensions
{
    public static void AddGame(this IServiceCollection services)
    {
        services.AddTransient<IMazeGenerator, MazeGeneratorAdapter>();
        
        services.AddTransient<ISystem, PlayerMovementSystem>();

        services.AddScoped<Renderer>();
        services.AddScoped<RenderPool>();

        services.AddScoped<EntityFactory>();

        services.AddScoped<EntityPool>();

        services.AddScoped<Game>();
    }
}
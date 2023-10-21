using Microsoft.Extensions.DependencyInjection;
using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Rendering;

namespace MysticEchoes.Core;

public static class Extensions
{
    public static void AddGame(this IServiceCollection services)
    {
        services.AddTransient<IMazeGenerator, MazeGeneratorAdapter>();

        services.AddScoped<EntityFactory>();

        services.AddScoped<ExecutableSystem, RenderSystem>();

        services.AddScoped(_ =>
        {
            var world = new World();

            world.RegisterComponentType<TileMapComponent>();
            world.RegisterComponentType<RenderComponent>();

            return world;
        });

        services.AddScoped<Game>();
    }
}
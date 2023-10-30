using Microsoft.Extensions.DependencyInjection;
using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;

namespace MysticEchoes.Core;

public static class Extensions
{
    public static void AddGame(this IServiceCollection services)
    {
        services.AddTransient<IMazeGenerator, MazeGeneratorAdapter>();

        services.AddScoped<EntityFactory>();

        services.AddScoped<ExecutableSystem, RenderSystem>();
        services.AddScoped<ExecutableSystem, TransformSystem>();

        services.AddScoped(_ =>
        {
            var world = new World();

            world.RegisterComponentType<TileMapComponent>();
            world.RegisterComponentType<RenderComponent>();
            world.RegisterComponentType<TransformComponent>();

            return world;
        });

        services.AddScoped<Game>();
    }
}
using Microsoft.Extensions.DependencyInjection;
using MysticEchoes.Core.MapModule;


namespace MysticEchoes.Core;

public static class Extensions
{
    public static void AddGame(this IServiceCollection services)
    {
        services.AddTransient<IMazeGenerator, MazeGeneratorAdapter>();

        services.AddScoped<Game>();
    }
}
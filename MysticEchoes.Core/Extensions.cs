using System.Numerics;
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

public static class Vector2Extensions
{
    private const float Epsilon = 0.001f;

    public static bool IsNearlyZero(this Vector2 vector)
    {
        return vector.LengthSquared() <= Epsilon * Epsilon;
    }
}
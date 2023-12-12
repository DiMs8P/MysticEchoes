using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using MysticEchoes.Core.MapModule;


namespace MysticEchoes.Core;
public static class Vector2Extensions
{
    private const float Epsilon = 0.001f;

    public static bool IsNearlyZero(this Vector2 vector)
    {
        return vector.LengthSquared() <= Epsilon * Epsilon;
    }
}
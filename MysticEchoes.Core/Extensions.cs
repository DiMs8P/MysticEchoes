using System.Numerics;
namespace MysticEchoes.Core;

public static class Vector2Extensions
{
    private const float Epsilon = 0.001f;

    public static bool IsNearlyZero(this Vector2 vector)
    {
        return vector.LengthSquared() <= Epsilon * Epsilon;
    }
    public static Vector2 ReflectionY(this Vector2 vector)
    {
        return new Vector2(vector.X, -vector.Y);
    }
    public static Vector2 Inverse(this Vector2 vector)
    { 
        return new Vector2(vector.Y, vector.X);
    }
}
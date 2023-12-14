using System.Numerics;
namespace MysticEchoes.Core;

public static class Vector2Extensions
{
    private const float Epsilon = 0.001f;

    public static bool IsNearlyZero(this Vector2 vector)
    {
        return vector.LengthSquared() <= Epsilon * Epsilon;
    }
    public static Vector2 ReflectY(this Vector2 vector)
    {
        return new Vector2(vector.X, -vector.Y);
    }
    public static Vector2 Inverse(this Vector2 vector)
    { 
        return new Vector2(vector.Y, vector.X);
    }
    
    public static float GetAngle(this Vector2 vector, Vector2 other)
    {
        return (float)(Math.Atan2(vector.Y - other.Y, vector.X - other.X) * (180 / Math.PI));
    }
}
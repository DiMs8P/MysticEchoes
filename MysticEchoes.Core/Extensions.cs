﻿using System.Numerics;
using SharpGL;

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
    
    public static float GetAngleBetweenGlobalX(this Vector2 vector)
    {
        double dotProduct = vector.X;
        double normA = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

        double cos = dotProduct / normA;
        int sign = vector.Y < 0 ? -1 : 1;
        double angle = (float)Math.Acos(cos) * sign;
        return (float)(angle * (180f / Math.PI));
    }
}

public static class OpenGlExtensions
{
    public static void Scale(this OpenGL gl, Vector2 scale)
    {
        gl.Scale(scale.X, scale.Y, 1);
    }
    
    public static void Translate(this OpenGL gl, Vector2 translation)
    {
        gl.Translate(translation.X, translation.Y, 0);
    }
    
    public static void Rotate(this OpenGL gl, float angle)
    {
        gl.Rotate(angle, 0, 0, 1);
    }
}

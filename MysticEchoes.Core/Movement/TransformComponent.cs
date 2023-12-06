using System.Numerics;

namespace MysticEchoes.Core.Movement;

public struct TransformComponent
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }

    public TransformComponent()
    {
        Position = Vector2.Zero;
    }
}
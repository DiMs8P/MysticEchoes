using System.Numerics;
using MysticEchoes.Core.Base.ECS;

namespace MysticEchoes.Core.Movement;

public class TransformComponent : Component
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }

    public TransformComponent()
    {
        Position = Vector2.Zero;
    }
}
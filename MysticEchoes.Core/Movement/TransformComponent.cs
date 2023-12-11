using System.Numerics;

namespace MysticEchoes.Core.Movement;

public struct TransformComponent
{
    public Vector2 Location { get; set; }
    public Vector2 Rotation { get; set; }
    public Vector2 Scale { get; set; }


    public TransformComponent()
    {
        Location = Vector2.Zero;
        Rotation = Vector2.Zero;
        Scale = Vector2.One;
    }
}
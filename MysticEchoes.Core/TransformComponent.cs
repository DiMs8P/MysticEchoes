using System.Numerics;
using MysticEchoes.Core.Base;

namespace MysticEchoes.Core;

public class TransformComponent : IComponent
{
    public Vector2 position { get; set; }
    public Vector2 velocity { get; set; }

    public TransformComponent()
    {
        position = Vector2.Zero;
        velocity = Vector2.Zero;
    }
}
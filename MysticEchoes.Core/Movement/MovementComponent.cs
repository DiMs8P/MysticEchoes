using System.Numerics;

namespace MysticEchoes.Core.Movement;

public struct MovementComponent
{
    public float Speed { get; set; }
    public MovementComponent()
    {
        Speed = 1f;
    }
}
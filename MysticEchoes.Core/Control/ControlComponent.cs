using System.Numerics;

namespace MysticEchoes.Core.Control;

public struct UnitControlComponent
{
    public Vector2 MoveDirection { get; set; }
    public bool IsShoot { get; set; }
    public Vector2 LookAt { get; set; }
}
using MysticEchoes.Core.Collisions.Tree;
using System.Numerics;

namespace MysticEchoes.Core.Collisions;

public struct DynamicCollider
{
    public Box Box { get; set; }
    public CollisionBehavior Behavior { get; set; }
    public Vector2 DefaultSize { get; set; }
}
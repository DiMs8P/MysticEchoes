using MysticEchoes.Core.Collisions.Tree;

namespace MysticEchoes.Core.Collisions;

public struct DynamicCollider
{
    public Box Box { get; set; }
    public CollisionBehavior Behavior { get; set; }
}
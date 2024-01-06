using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;

namespace MysticEchoes.Core.AI.Ecs;

public class EcsNode : Node
{
    protected EcsWorld World;
    protected int SelfEntityId;

    protected EcsNode(EcsWorld world, int selfEntityId)
    {
        World = world;
        SelfEntityId = selfEntityId;
    }
}
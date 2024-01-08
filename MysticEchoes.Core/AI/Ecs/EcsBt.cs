using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;

namespace MysticEchoes.Core.AI.Ecs;

public abstract class EcsBt : Tree
{
    protected EcsBt(EcsWorld world, int ownerEntityId)
    {
        Blackboard.SetValueAsInt("SelfEntityId", ownerEntityId);
        Blackboard.SetValueAsObject("World", world);
    }
}
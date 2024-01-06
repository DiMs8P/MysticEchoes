using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;

namespace MysticEchoes.Core.AI.Ecs;

public abstract class EcsBt : Tree
{
    protected readonly EcsWorld _world;
    protected readonly int _ownerEntityId;
    protected EcsBt(EcsWorld world, int ownerEntityId)
    {
        _world = world;
        _ownerEntityId = ownerEntityId;
    }
}
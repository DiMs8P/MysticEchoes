using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Player;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.AI.Tasks;

public class Attack : Node
{
    private readonly EcsWorld _world;

    private readonly EcsPool<TransformComponent> _transforms;
    private readonly EcsPool<RangeWeaponComponent> _weapons;
    private readonly int _playerEntityId;
    private readonly int _ownerEntityId;
    public Attack(EcsWorld world, int ownerEntityId)
    {
        _world = world;
        _ownerEntityId = ownerEntityId;
        _transforms = _world.GetPool<TransformComponent>();

        EcsFilter playerFilter = world.Filter<PlayerMarker>().End();

        foreach (var playerEntityId in playerFilter)
        {
            _playerEntityId = playerEntityId;
        }
    }

    public override NodeState Evaluate()
    {
        ref RangeWeaponComponent rangeWeaponComponent = ref _weapons.Get(_ownerEntityId);
        rangeWeaponComponent.IsShooting = true;

        return NodeState.Success;
    }
}
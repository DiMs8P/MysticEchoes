using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Player;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.AI.Tasks;

public class Attack : EcsNode
{
    private readonly EcsPool<TransformComponent> _transforms;
    private readonly EcsPool<RangeWeaponComponent> _weapons;
    
    public Attack(EcsWorld world, int selfEntityId) : base(world, selfEntityId)
    {
        _transforms = World.GetPool<TransformComponent>();
        _weapons = World.GetPool<RangeWeaponComponent>();
    }

    public override NodeState Evaluate()
    {
        if ((bool)GetData("HasAim"))
        {
            ref RangeWeaponComponent rangeWeaponComponent = ref _weapons.Get(SelfEntityId);
            rangeWeaponComponent.IsShooting = true;
            return NodeState.Success;
        }
        else
        {
            ref RangeWeaponComponent rangeWeaponComponent = ref _weapons.Get(SelfEntityId);
            rangeWeaponComponent.IsShooting = false;
            return NodeState.Failure;
        }
    }
}
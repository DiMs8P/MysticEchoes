using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.AI.Tasks;

public class Attack : EcsNode
{
    private readonly EcsPool<RangeWeaponComponent> _weapons;
    
    public Attack(Blackboard blackboard) : base(blackboard)
    {
        _weapons = World.GetPool<RangeWeaponComponent>();
    }

    public override NodeState Evaluate()
    {
        bool hasAim = Blackboard.GetValueAsBool("HasAim");
        if (hasAim)
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
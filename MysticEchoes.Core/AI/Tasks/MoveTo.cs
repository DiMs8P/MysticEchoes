using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Control;
using MysticEchoes.Core.Movement;

namespace MysticEchoes.Core.AI.Tasks;

public class MoveTo : EcsNode
{
    protected int _target;

    private EcsPool<TransformComponent> _transforms;
    private EcsPool<UnitControlComponent> _controls;
    
    public MoveTo(EcsWorld world, int ownerEntityId, int target) : base(world, ownerEntityId)
    {
        _transforms = World.GetPool<TransformComponent>();
        _controls = World.GetPool<UnitControlComponent>();
        _target = target;
    }

    public override NodeState Evaluate()
    {
        ref TransformComponent ownerTransform = ref _transforms.Get(SelfEntityId);
        ref TransformComponent targetTransform = ref _transforms.Get(_target);

        Vector2 targetVector = targetTransform.Location - ownerTransform.Location;
        if (targetVector.Length() < 0.0001)
        {
            return NodeState.Success;
        }
        else
        {
            ref UnitControlComponent controlComponent = ref _controls.Get(SelfEntityId);
            controlComponent.LookAt = targetTransform.Location;
            controlComponent.MoveDirection = targetTransform.Location - ownerTransform.Location;

            return NodeState.Running;
        }
    }
}
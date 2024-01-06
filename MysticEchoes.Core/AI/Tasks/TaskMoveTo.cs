using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Control;
using MysticEchoes.Core.Movement;

namespace MysticEchoes.Core.AI.Tasks;

public class TaskMoveTo : EcsNode
{
    protected int _target;
    protected float _accuracy;

    private EcsPool<TransformComponent> _transforms;
    private EcsPool<UnitControlComponent> _controls;
    
    public TaskMoveTo(EcsWorld world, int selfEntityId, int target, float accuracy) : base(world, selfEntityId)
    {
        _transforms = World.GetPool<TransformComponent>();
        _controls = World.GetPool<UnitControlComponent>();
        
        _target = target;
        _accuracy = accuracy;
    }

    public override NodeState Evaluate()
    {
        ref TransformComponent ownerTransform = ref _transforms.Get(SelfEntityId);
        ref TransformComponent targetTransform = ref _transforms.Get(_target);

        Vector2 targetVector = targetTransform.Location - ownerTransform.Location;
        if (targetVector.Length() > _accuracy)
        {
            ref UnitControlComponent controlComponent = ref _controls.Get(SelfEntityId);
            controlComponent.LookAt = targetTransform.Location;
            controlComponent.MoveDirection = targetTransform.Location - ownerTransform.Location;
        }

        State = NodeState.Running;
        return NodeState.Running;
    }
}
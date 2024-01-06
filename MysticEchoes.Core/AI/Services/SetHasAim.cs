using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Movement;

namespace MysticEchoes.Core.AI.Services;

public class SetHasAim : EcsNode
{
    protected int _target;
    protected float _accuracy;

    private EcsPool<TransformComponent> _transforms;
    public SetHasAim(EcsWorld world, int selfEntityId, int target, float accuracy) : base(world, selfEntityId)
    {
        _transforms = World.GetPool<TransformComponent>();
        
        _target = target;
        _accuracy = accuracy;
    }

    public override NodeState Evaluate()
    {
        ref TransformComponent ownerTransform = ref _transforms.Get(SelfEntityId);
        ref TransformComponent targetTransform = ref _transforms.Get(_target);

        Vector2 targetVector = targetTransform.Location - ownerTransform.Location;
        Parent.SetData("HasAim", targetVector.Length() <= _accuracy);

        State = NodeState.Success;
        return State;
    }
}
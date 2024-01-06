using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Movement;

namespace MysticEchoes.Core.AI.Decorators;

public class CheckEnemyInRange : EcsNode
{
    private EcsPool<TransformComponent> _transforms;
    
    protected int _target;
    protected float _accuracy;
    public CheckEnemyInRange(EcsWorld world, int selfEntityId, int target, float accuracy) : base(world, selfEntityId)
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
        if (targetVector.Length() > _accuracy)
        {
            State = NodeState.Success;
            return State;
        }

        State = NodeState.Failure;
        return State;
    }
}
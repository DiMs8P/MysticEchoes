using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Movement;

namespace MysticEchoes.Core.AI.Decorators;

public class CheckEnemyInRange : EcsNode
{
    private EcsPool<TransformComponent> _transforms;

    private string _targetKey;
    private float _accuracy;
    public CheckEnemyInRange(Blackboard blackboard, string targetKey, float accuracy) : base(blackboard)
    {
        _transforms = World.GetPool<TransformComponent>();
        
        _targetKey = targetKey;
        _accuracy = accuracy;
    }

    public override NodeState Evaluate()
    {
        int targetId = Blackboard.GetValueAsInt(_targetKey);
        if (targetId != -1)
        {
            ref TransformComponent ownerTransform = ref _transforms.Get(SelfEntityId);
            ref TransformComponent targetTransform = ref _transforms.Get(targetId);

            Vector2 targetVector = targetTransform.Location - ownerTransform.Location;
            if (targetVector.Length() > _accuracy)
            {
                State = NodeState.Success;
                return State;
            }
        }
        
        State = NodeState.Failure;
        return State;
    }
}
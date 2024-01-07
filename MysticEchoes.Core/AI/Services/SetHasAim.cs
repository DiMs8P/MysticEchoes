using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Movement;

namespace MysticEchoes.Core.AI.Services;

public class SetHasAim : EcsNode
{
    private string _target;
    private float _accuracy;

    private EcsPool<TransformComponent> _transforms;
    public SetHasAim(Blackboard blackboard, string target, float accuracy) : base(blackboard)
    {
        _transforms = World.GetPool<TransformComponent>();
        
        _target = target;
        _accuracy = accuracy;
    }

    public override NodeState Evaluate()
    {
        int target = Blackboard.GetValueAsInt(_target);
        if (target != -1)
        {
            ref TransformComponent ownerTransform = ref _transforms.Get(SelfEntityId);
            ref TransformComponent targetTransform = ref _transforms.Get(target);

            Vector2 targetVector = targetTransform.Location - ownerTransform.Location;
            Blackboard.SetValueAsBool("HasAim", targetVector.Length() <= _accuracy);

            State = NodeState.Success;
            return State;
        }
        
        State = NodeState.Failure;
        return State;
    }
}
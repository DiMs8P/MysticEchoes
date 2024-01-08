using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Control;
using MysticEchoes.Core.Movement;

namespace MysticEchoes.Core.AI.Tasks;

public class TaskMoveTo : EcsNode
{
    private string _target;
    private float _accuracy;

    private EcsPool<TransformComponent> _transforms;
    private EcsPool<UnitControlComponent> _controls;
    
    public TaskMoveTo(Blackboard blackboard, string targetKey, float accuracy) : base(blackboard)
    {
        _transforms = World.GetPool<TransformComponent>();
        _controls = World.GetPool<UnitControlComponent>();
        
        _target = targetKey;
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
            if (targetVector.Length() > _accuracy)
            {
                ref UnitControlComponent controlComponent = ref _controls.Get(SelfEntityId);
                controlComponent.LookAt = targetTransform.Location;
                controlComponent.MoveDirection = targetTransform.Location - ownerTransform.Location;
                State = NodeState.Running;
            }
            else
            {
                ref UnitControlComponent controlComponent = ref _controls.Get(SelfEntityId);
                controlComponent.LookAt = targetTransform.Location;
                controlComponent.MoveDirection = Vector2.Zero;
                State = NodeState.Success;
            }
        }
        else
        {
            State = NodeState.Failure;
        }

        return State;
    }
}
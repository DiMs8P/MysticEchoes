using Leopotam.EcsLite;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;

namespace MysticEchoes.Core.Animations.StateMachines;

public class PlayerStateMachine : BaseStateMachine
{
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    public PlayerStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
        _animations = world.GetPool<AnimationComponent>();
        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
        _movements = world.GetPool<MovementComponent>();
        _transforms = world.GetPool<TransformComponent>();
    }

    public override void Update()
    {
        ref AnimationComponent animationComponent = ref _animations.Get(OwnerEntityId);
        ref CharacterAnimationComponent characterAnimationComponent = ref _characterAnimations.Get(OwnerEntityId);

        CharacterState currentState = GetCurrentState();

        if (characterAnimationComponent.Animations.TryGetValue(currentState, out var animation) && _animations.Has(OwnerEntityId))
        {
            ref AnimationComponent playerAnimation = ref _animations.Get(OwnerEntityId);
            playerAnimation.AnimationId = animation;
            if (currentState != characterAnimationComponent.CurrentState)
            {
                characterAnimationComponent.CurrentState = currentState;
                animationComponent.CurrentFrameIndex = 0;
            }
        }
    }

    public override CharacterState GetCurrentState()
    {
        ref var transform = ref _transforms.Get(OwnerEntityId);
        ref var movement = ref _movements.Get(OwnerEntityId);

        CharacterState state = !movement.Velocity.IsNearlyZero()
            ? transform.Rotation.X > transform.Rotation.Y
            ? transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.MovingRight : CharacterState.MovingDown
            : transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.MovingTop : CharacterState.MovingLeft
            : transform.Rotation.X > transform.Rotation.Y
            ? transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.StayRight : CharacterState.Idle
            : transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.StayTop : CharacterState.StayLeft;

        return state;
    }
}
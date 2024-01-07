using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.Animations.StateMachines;

public class BringerOfDeathStateMachine : BaseStateMachine
{
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<RangeWeaponComponent> _weapons;
    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    public BringerOfDeathStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
        _animations = world.GetPool<AnimationComponent>();
        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
        _movements = world.GetPool<MovementComponent>();
        _transforms = world.GetPool<TransformComponent>();
        _weapons = world.GetPool<RangeWeaponComponent>();
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
            
            animationComponent.ReflectByY = Reflect();
            if (currentState != characterAnimationComponent.CurrentState)
            {
                characterAnimationComponent.CurrentState = currentState;
                animationComponent.CurrentFrameIndex = 0;
            }
        }
    }

    public override CharacterState GetCurrentState()
    {
        ref var movement = ref _movements.Get(OwnerEntityId);
        ref var weapon = ref _weapons.Get(OwnerEntityId);

        CharacterState state = weapon.IsShooting
            ? CharacterState.Shooting
            : !movement.Velocity.IsNearlyZero() ? CharacterState.Run : CharacterState.Idle;

        return state;
    }
    public bool Reflect()
    {
        ref var transform = ref _transforms.Get(OwnerEntityId);

        return transform.Rotation.X > 0;
    }
}
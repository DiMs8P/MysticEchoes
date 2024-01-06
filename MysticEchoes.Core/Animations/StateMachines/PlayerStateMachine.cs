using Leopotam.EcsLite;

namespace MysticEchoes.Core.Animations.StateMachines;

public class PlayerStateMachine : BaseStateMachine
{
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    public PlayerStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
        _animations = world.GetPool<AnimationComponent>();
        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
    }

    public override void Update()
    {
        ref AnimationComponent animationComponent = ref _animations.Get(OwnerEntityId);
        ref CharacterAnimationComponent characterAnimationComponent = ref _characterAnimations.Get(OwnerEntityId);

        CharacterState currentState = GetCurrentState();
        
        if (currentState is CharacterState.Idle)
        {
            animationComponent.AnimationId = characterAnimationComponent.Animations[CharacterState.Idle];
        }
    }

    public override CharacterState GetCurrentState()
    {
        return CharacterState.Idle;
    }
}
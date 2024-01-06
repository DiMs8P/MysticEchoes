using Leopotam.EcsLite;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Player;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Animations;

public class PlayerAnimationSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private AnimationManager _animationManager;

    private EcsFilter _playerFilter;

    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<SpriteComponent> _sprites;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().End();

        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
        _animations = world.GetPool<AnimationComponent>();
        _sprites = world.GetPool<SpriteComponent>();
    }

    // TODO implement
    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref var transform = ref _transforms.Get(playerId);
            ref CharacterAnimationComponent playerAnimationComponent = ref _characterAnimations.Get(playerId);

            ref MovementComponent movement = ref _movements.Get(playerId);

            if (!movement.Velocity.IsNearlyZero())
            {
                playerAnimationComponent.CurrentState = transform.Rotation.X > transform.Rotation.Y
                ? transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.MovingRight : CharacterState.MovingDown
                : transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.MovingTop : CharacterState.MovingLeft;
            }
            else
            {
                playerAnimationComponent.CurrentState = transform.Rotation.X > transform.Rotation.Y
                ? transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.StayRight : CharacterState.Idle
                : transform.Rotation.X > (-1) * transform.Rotation.Y ? CharacterState.StayTop : CharacterState.StayLeft;
            }
                

            if (playerAnimationComponent.Animations.TryGetValue(playerAnimationComponent.CurrentState, out var animation) && _animations.Has(playerId))
            {
                ref AnimationComponent playerAnimation = ref _animations.Get(playerId);
                playerAnimation.AnimationId = animation;
            }
        }
    }
}
using Leopotam.EcsLite;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Player;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Animations;

public class PlayerAnimationSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;

    private EcsFilter _playerFilter;
    
    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<SpriteComponent> _sprites;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().End();

        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
        _animations = world.GetPool<AnimationComponent>();
        _sprites = world.GetPool<SpriteComponent>();
    }

    // TODO implement
    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            /*ref AnimationComponent playerFrames = ref _animations.Get(playerId);
            ref SpriteComponent playerSprite = ref _sprites.Get(playerId);
            
            if (_inputManager.GetVertical() == 0 && _inputManager.GetHorizontal() == 0)
            {
                playerFrames.IsActive = false;
                playerFrames.CurrentFrameIndex = 0;
                playerFrames.CurrentFrameElapsedTime = 0;

                playerSprite.Sprite = playerFrames.Frames[playerFrames.CurrentFrameIndex].Sprite;
            }
            else
            {
                playerFrames.IsActive = true;
            }*/
        }
    }
}
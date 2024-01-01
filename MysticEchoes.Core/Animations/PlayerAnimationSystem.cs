using Leopotam.EcsLite;
using MysticEchoes.Core.Config.Input;
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
        }
    }
}
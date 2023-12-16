using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class PlayerSpawnerSystem : IEcsInitSystem
{
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private EntityFactory _factory;

    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<SpriteComponent> _sprites;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
        _animations = world.GetPool<AnimationComponent>();
        _sprites = world.GetPool<SpriteComponent>();
        
        CreatePlayer(_factory);
    }

    private int CreatePlayer(EntityFactory factory)
    {
        int player = _prefabManager.CreateEntityFromPrefab(factory, PrefabType.Player);

        SetupPlayerAnimations(player);
        SetupPlayerSprite(player);

        return player;
    }

    private void SetupPlayerAnimations(int playerId)
    {
        if (_characterAnimations.Has(playerId))
        {
            ref CharacterAnimationComponent playerAnimationComponent = ref _characterAnimations.Get(playerId);

            if (playerAnimationComponent.Animations.TryGetValue(playerAnimationComponent.InitialState, out var animation) && _animations.Has(playerId))
            {
                ref AnimationComponent playerAnimation = ref _animations.Get(playerId);
                playerAnimation.Frames = animation;
            }
            else
            {
                throw new ArgumentException("Prefab setup error");
            }
        }
    }
    private void SetupPlayerSprite(int playerId)
    {
        if (_animations.Has(playerId))
        {
            ref AnimationComponent playerAnimationComponent = ref _animations.Get(playerId);

            if (playerAnimationComponent.Frames.Count() > playerAnimationComponent.CurrentFrameIndex && _sprites.Has(playerId))
            {
                ref SpriteComponent playerAnimation = ref _sprites.Get(playerId);
                playerAnimation.Sprite = playerAnimationComponent.Frames[playerAnimationComponent.CurrentFrameIndex].Sprite;
            }
            else
            {
                throw new ArgumentException("Prefab setup error");
            }
        }
    }
}
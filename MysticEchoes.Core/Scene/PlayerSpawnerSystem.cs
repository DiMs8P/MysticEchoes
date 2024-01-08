using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Animations.StateMachines;
using MysticEchoes.Core.Inventory;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Health;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Camera;

namespace MysticEchoes.Core.Scene;

public class PlayerSpawnerSystem : IEcsInitSystem
{
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private AnimationManager _animationManager;
    [EcsInject] private EntityBuilder _builder;
    [EcsInject] private ItemsFactory _itemsFactory;

    private EcsWorld _world;
    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<SpriteComponent> _sprites;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<DynamicCollider> _colliders;
    private EcsPool<HealthComponent> _health;
    private EcsPool<CameraComponent> _camera;
    
    private EcsPool<RangeWeaponComponent> _weapons;
    private EcsPool<OwningByComponent> _ownings;
    
    private EcsPool<StartingItems> _items;
    private EcsPool<TileMapComponent> _maps;
    private int _mapId;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _characterAnimations = _world.GetPool<CharacterAnimationComponent>();
        _animations = _world.GetPool<AnimationComponent>();
        _sprites = _world.GetPool<SpriteComponent>();
        _transforms = _world.GetPool<TransformComponent>();
        _colliders = _world.GetPool<DynamicCollider>();
        _health = _world.GetPool<HealthComponent>();
        _camera = _world.GetPool<CameraComponent>();

        _weapons = _world.GetPool<RangeWeaponComponent>();
        _ownings = _world.GetPool<OwningByComponent>();

        _items = _world.GetPool<StartingItems>();
        _maps = _world.GetPool<TileMapComponent>();

        var mapFilter = _world.Filter<TileMapComponent>()
            .End();
        foreach (var mapId in mapFilter)
        {
            _mapId = mapId;
        }
        CreatePlayer(_builder);
    }

    private int CreatePlayer(EntityBuilder builder)
    {
        int player = _prefabManager.CreateEntityFromPrefab(builder, PrefabType.Player);
        int playerWeapon = _prefabManager.CreateEntityFromPrefab(builder, PrefabType.DefaultWeapon);

        SetupPositions(player);
        SetupCamera(player);
        SetupPlayerAnimations(player);
        SetupPlayerSprite(player);
        SetupCollider(player);
        SetupPlayerHealth(player);

        SetupPlayerWeapon(player, playerWeapon);
        SetupPlayerStarterItems(player);

        return player;
    }

    private void SetupPositions(int player)
    {
        var map = _maps.Get(_mapId);
        var spawn = map.Map.PlayerSpawn;

        ref var transform = ref _transforms.Get(player);
        transform.Location = new Vector2(
            spawn.X * map.TileSize.X,
            spawn.Y * map.TileSize.Y
        );

    }
    private void SetupCamera(int player)
    {
        ref var transform = ref _transforms.Get(player);
        ref var camera = ref _camera.Get(player);

        camera.Position = new Vector2(transform.Location.X - 1, transform.Location.Y - 1);
    }
    private void SetupCollider(int player)
    {
        ref var playerCollider = ref _colliders.Get(player);
        ref var transform = ref _transforms.Get(player);
        //transform.Scale /= 9;

        playerCollider.Box = new Box(player, new Rectangle(
            Vector2.Zero, 
            playerCollider.DefaultSize * transform.Scale
        ));
        playerCollider.Behavior = CollisionBehavior.AllyCharacter;
    }

    private void SetupPlayerAnimations(int playerId)
    {
        if (_characterAnimations.Has(playerId))
        {
            ref CharacterAnimationComponent playerAnimationComponent = ref _characterAnimations.Get(playerId);
            playerAnimationComponent.AnimationStateMachine = new PlayerStateMachine(playerId, _world);
            playerAnimationComponent.CurrentState = CharacterState.Idle;

            if (playerAnimationComponent.Animations.TryGetValue(playerAnimationComponent.CurrentState, out var animation) && _animations.Has(playerId))
            {
                ref AnimationComponent playerAnimation = ref _animations.Get(playerId);
                playerAnimation.AnimationId = animation;
            }
            else
            {
                throw new ArgumentException("Player prefab must have Animation component if he has CharacterAnimationComponent and " +
                                            "have animations for its initial state");
            }
        }
    }
    private void SetupPlayerSprite(int playerId)
    {
        if (_animations.Has(playerId))
        {
            ref AnimationComponent playerAnimationComponent = ref _animations.Get(playerId);
            AnimationFrame[] playerAnimations = _animationManager.GetAnimationFrames(playerAnimationComponent.AnimationId);
            if (playerAnimations.Count() > playerAnimationComponent.CurrentFrameIndex && _sprites.Has(playerId))
            {
                ref SpriteComponent playerAnimation = ref _sprites.Get(playerId);
                playerAnimation.Sprite = playerAnimations[playerAnimationComponent.CurrentFrameIndex].Sprite;
            }
            else
            {
                throw new ArgumentException("Player prefab must have Sprite component if he has AnimationComponent and " +
                                            "have initial sprite to render");
            }
        }
    }
    
    private void SetupPlayerHealth(int player)
    {
        ref HealthComponent playerHealth = ref _health.Get(player);
        playerHealth.Health = playerHealth.MaxHealth;
    }
    
    private void SetupPlayerWeapon(int player, int playerWeapon)
    {
        ref RangeWeaponComponent rangeWeaponComponent = ref _weapons.Get(player);
        rangeWeaponComponent.MuzzleIds.Add(playerWeapon);

        ref OwningByComponent owningByComponent = ref _ownings.Get(playerWeapon);
        owningByComponent.Owner = player;
    }
    
    private void SetupPlayerStarterItems(int player)
    {
        if (_items.Has(player))
        {
            ref StartingItems givenStartItems = ref _items.Get(player);

            foreach (int item in givenStartItems.Items)
            {
                BaseItem startItem = _itemsFactory.CreateItem(item);
                startItem.OnItemTaken(player, _world);
            }
            
            _items.Del(player);
        }
    }
}
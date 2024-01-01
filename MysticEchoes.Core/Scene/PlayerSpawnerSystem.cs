using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Inventory;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class PlayerSpawnerSystem : IEcsInitSystem
{
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private AnimationManager _animationManager;
    [EcsInject] private EntityFactory _factory;

    private EcsPool<CharacterAnimationComponent> _characterAnimations;
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<SpriteComponent> _sprites;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<DynamicCollider> _colliders;
    private EcsPool<MovementComponent> _movements;
    
    private EcsPool<RangeWeaponComponent> _weapons;
    private EcsPool<OwningByComponent> _ownings;
    
    private EcsPool<StartingItems> _items;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
        _animations = world.GetPool<AnimationComponent>();
        _sprites = world.GetPool<SpriteComponent>();
        _transforms = world.GetPool<TransformComponent>();
        _colliders = world.GetPool<DynamicCollider>();
        _movements = world.GetPool<MovementComponent>();

        _weapons = world.GetPool<RangeWeaponComponent>();
        _ownings = world.GetPool<OwningByComponent>();

        _items = world.GetPool<StartingItems>();
        
        CreatePlayer(_factory, world);
    }

    private int CreatePlayer(EntityFactory factory, EcsWorld world)
    {
        int player = _prefabManager.CreateEntityFromPrefab(factory, PrefabType.Player);
        int playerWeapon = _prefabManager.CreateEntityFromPrefab(factory, PrefabType.DefaultWeapon);
        
        SetupPlayerAnimations(player);
        SetupPlayerSprite(player);
        SetupCollider(player);

        SetupPlayerWeapon(player, playerWeapon);
        SetupPlayerStarterItems(player, world);

        return player;
    }

    private void SetupCollider(int player)
    {
        ref var playerCollider = ref _colliders.Get(player);
        ref var transform = ref _transforms.Get(player);
        //transform.Scale /= 9;

        playerCollider.Box = new Box(player, new Rectangle(
            Vector2.Zero, 
            new Vector2(0.265f, 0.35f) * transform.Scale
        ));
        playerCollider.Behavior = CollisionBehavior.AllyCharacter;
    }

    private void SetupPlayerAnimations(int playerId)
    {
        if (_characterAnimations.Has(playerId))
        {
            ref CharacterAnimationComponent playerAnimationComponent = ref _characterAnimations.Get(playerId);
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
    
    private void SetupPlayerWeapon(int player, int playerWeapon)
    {
        ref RangeWeaponComponent rangeWeaponComponent = ref _weapons.Get(player);
        rangeWeaponComponent.MuzzleIds.Add(playerWeapon);

        ref OwningByComponent owningByComponent = ref _ownings.Get(playerWeapon);
        owningByComponent.Owner = player;
    }
    
    private void SetupPlayerStarterItems(int player, EcsWorld world)
    {
        if (_items.Has(player))
        {
            ref StartingItems givenStartItems = ref _items.Get(player);

            foreach (Item item in givenStartItems.Items)
            {
                BaseItem startItem = ItemsFactory.CreateItem(item);
                startItem.OnItemTaken(player, world);
            }
            
            _items.Del(player);
        }
    }
}
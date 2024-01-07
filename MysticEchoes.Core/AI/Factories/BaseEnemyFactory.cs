using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Animations.StateMachines;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Health;
using MysticEchoes.Core.Inventory;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Scene;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.AI.Factories;

public class BaseEnemyFactory : IEnemyFactory
{
    protected EcsWorld World;
    protected EntityBuilder Builder;
    protected ItemsFactory ItemsFactory;
    protected PrefabManager PrefabManager;

    protected EcsPool<HealthComponent> _health;
    protected EcsPool<EnemyComponent> _enemies;
    protected EcsPool<DynamicCollider> _colliders;
    protected EcsPool<TransformComponent> _transforms;
    protected EcsPool<CharacterAnimationComponent> _animations;
    
    protected EcsPool<RangeWeaponComponent> _weapons;
    protected EcsPool<OwningByComponent> _ownings;
    protected EcsPool<StartingItems> _items;
    
    public BaseEnemyFactory(EcsWorld world, EntityBuilder builder, ItemsFactory itemsFactory, PrefabManager prefabManager)
    {
        World = world;
        Builder = builder;
        ItemsFactory = itemsFactory;
        PrefabManager = prefabManager;

        _health = world.GetPool<HealthComponent>();
        _enemies = world.GetPool<EnemyComponent>();
        _colliders = world.GetPool<DynamicCollider>();
        _transforms = world.GetPool<TransformComponent>();
        _animations = world.GetPool<CharacterAnimationComponent>();

        _weapons = world.GetPool<RangeWeaponComponent>();
        _ownings = world.GetPool<OwningByComponent>();
        _items = world.GetPool<StartingItems>();
    }

    public virtual int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        EnemyInitializationInternalInfo enemyInitializationInternalInfo = new EnemyInitializationInternalInfo();
        enemyInitializationInternalInfo.EnemyWeaponPrefab = PrefabType.None;
        enemyInitializationInternalInfo.EnemyPrefab = PrefabType.None;
        int createdEnemy = CreateInternal(enemyInitializationInfo, enemyInitializationInternalInfo);
        
        return createdEnemy;
    }

    protected virtual int CreateInternal(EnemyInitializationInfo enemyInitializationInfo, EnemyInitializationInternalInfo enemyInitializationInternalInfo)
    {
        int createdEnemy = PrefabManager.CreateEntityFromPrefab(Builder, enemyInitializationInternalInfo.EnemyPrefab);
        InitializeEnemy(createdEnemy, enemyInitializationInfo);
        InitializeEnemyWeapon(createdEnemy, enemyInitializationInternalInfo);
        InitializeEnemyInventoryItems(createdEnemy);

        return createdEnemy;
    }

    protected virtual void InitializeEnemy(int createdEnemyId, EnemyInitializationInfo enemyInitializationInfo)
    {
        ref TransformComponent transformComponent = ref _transforms.Get(createdEnemyId);
        transformComponent.Location = enemyInitializationInfo.Location;
        
        ref DynamicCollider dynamicCollider = ref _colliders.Get(createdEnemyId);
        dynamicCollider = new DynamicCollider()
        {
            Box = new Box(createdEnemyId, new Rectangle(
                Vector2.Zero,
                new Vector2(0.265f, 0.35f) * transformComponent.Scale
            ))
        };
        dynamicCollider.Behavior = CollisionBehavior.EnemyCharacter;
        
        ref HealthComponent enemyHealth = ref _health.Get(createdEnemyId);
        enemyHealth.Health = enemyHealth.MaxHealth;

        ref EnemyComponent enemyComponent = ref _enemies.Get(createdEnemyId);
        enemyComponent.EnemyId = enemyInitializationInfo.EnemyId;
        enemyComponent.RoomId = enemyInitializationInfo.RoomId;
    }
    
    protected virtual void InitializeEnemyWeapon(int createdEnemyId, EnemyInitializationInternalInfo enemyInitializationInternalInfo)
    {
        int playerWeapon = PrefabManager.CreateEntityFromPrefab(Builder, enemyInitializationInternalInfo.EnemyWeaponPrefab);
        
        ref RangeWeaponComponent rangeWeaponComponent = ref _weapons.Get(createdEnemyId);
        rangeWeaponComponent.MuzzleIds.Add(playerWeapon);

        ref OwningByComponent owningByComponent = ref _ownings.Get(playerWeapon);
        owningByComponent.Owner = createdEnemyId;
    }
    
    protected virtual void InitializeEnemyInventoryItems(int createdEnemy)
    {
        if (_items.Has(createdEnemy))
        {
            ref StartingItems givenStartItems = ref _items.Get(createdEnemy);

            foreach (int item in givenStartItems.Items)
            {
                BaseItem startItem = ItemsFactory.CreateItem(item);
                startItem.OnItemTaken(createdEnemy, World);
            }
            
            _items.Del(createdEnemy);
        }
    }
}
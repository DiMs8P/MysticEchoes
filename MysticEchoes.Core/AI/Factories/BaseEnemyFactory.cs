using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Events;
using MysticEchoes.Core.Health;
using MysticEchoes.Core.Inventory;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.MapModule.Rooms;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.AI.Factories;

public class BaseEnemyFactory : IEnemyFactory
{
    protected EcsWorld World;
    protected EntityBuilder Builder;
    protected ItemsFactory ItemsFactory;
    protected PrefabManager PrefabManager;

    protected EcsPool<AiComponent> _ai;
    protected EcsPool<HealthComponent> _health;
    protected EcsPool<EnemyComponent> _enemies;
    protected EcsPool<SpriteComponent> _sprites;
    protected EcsPool<DynamicCollider> _colliders;
    protected EcsPool<TransformComponent> _transforms;
    protected EcsPool<AnimationComponent> _animations;
    protected EcsPool<CharacterAnimationComponent> _characterAnimations;
    
    protected EcsPool<RangeWeaponComponent> Weapons;
    protected EcsPool<OwningByComponent> _ownings;
    protected EcsPool<StartingItems> _items;
    protected EcsPool<MuzzleComponent> Muzzles;

    public BaseEnemyFactory(EcsWorld world, EntityBuilder builder, ItemsFactory itemsFactory, PrefabManager prefabManager)
    {
        World = world;
        Builder = builder;
        ItemsFactory = itemsFactory;
        PrefabManager = prefabManager;

        _ai = world.GetPool<AiComponent>();
        _health = world.GetPool<HealthComponent>();
        _enemies = world.GetPool<EnemyComponent>();
        _sprites = world.GetPool<SpriteComponent>();
        _colliders = world.GetPool<DynamicCollider>();
        _transforms = world.GetPool<TransformComponent>();
        _animations = world.GetPool<AnimationComponent>();
        _characterAnimations = world.GetPool<CharacterAnimationComponent>();
        
        Weapons = world.GetPool<RangeWeaponComponent>();
        _ownings = world.GetPool<OwningByComponent>();
        _items = world.GetPool<StartingItems>();
        Muzzles = world.GetPool<MuzzleComponent>();
    }

    public virtual int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        throw new NotImplementedException("Method must be implemented");
    }

    protected virtual int CreateInternal(EnemyInitializationInfo enemyInitializationInfo, EnemyInitializationInternalInfo enemyInitializationInternalInfo)
    {
        int createdEnemy = PrefabManager.CreateEntityFromPrefab(Builder, enemyInitializationInternalInfo.EnemyPrefab);
        InitializeEnemy(createdEnemy, enemyInitializationInfo);
        InitializeEnemyAi(createdEnemy, enemyInitializationInternalInfo);
        InitializeEnemyWeapon(createdEnemy, enemyInitializationInternalInfo);
        InitializeEnemyAnimations(createdEnemy, enemyInitializationInternalInfo);
        InitializeEnemyInventoryItems(createdEnemy);

        return createdEnemy;
    }

    protected virtual void InitializeEnemy(int createdEnemyId, EnemyInitializationInfo enemyInitializationInfo)
    {
        ref TransformComponent transformComponent = ref _transforms.Get(createdEnemyId);
        transformComponent.Location = enemyInitializationInfo.Location;
        
        ref DynamicCollider dynamicCollider = ref _colliders.Get(createdEnemyId);
        dynamicCollider.Box = new Box(createdEnemyId, new Rectangle(
                Vector2.Zero,
                dynamicCollider.DefaultSize * transformComponent.Scale
            ));
           
        dynamicCollider.Behavior = CollisionBehavior.EnemyCharacter;
        
        ref HealthComponent enemyHealth = ref _health.Get(createdEnemyId);
        enemyHealth.Health = enemyHealth.MaxHealth;
        enemyHealth.OnPreDeadEvent += OnDeadEventHandler;
        
        ref EnemyComponent enemyComponent = ref _enemies.Get(createdEnemyId);
        enemyComponent.EnemyId = enemyInitializationInfo.EnemyId;
        enemyComponent.RoomId = enemyInitializationInfo.RoomId;
    }
    
    protected virtual void InitializeEnemyAi(int createdEnemyId, EnemyInitializationInternalInfo enemyInitializationInternalInfo)
    {
        ref AiComponent aiComponent = ref _ai.Get(createdEnemyId);
        EcsBt? behaviorTree = Activator.CreateInstance(enemyInitializationInternalInfo.EnemyBehaviorTree, World, createdEnemyId) as EcsBt;
        if (behaviorTree is null)
        {
            throw new ArgumentException("Can't create behavior tree instance");
        }

        aiComponent.BehaviorTree = behaviorTree;
        aiComponent.BehaviorTree.Start();
    }
    
    protected virtual void InitializeEnemyWeapon(int createdEnemyId, EnemyInitializationInternalInfo enemyInitializationInternalInfo)
    {
        int playerWeapon = PrefabManager.CreateEntityFromPrefab(Builder, enemyInitializationInternalInfo.EnemyWeaponPrefab);
        
        ref RangeWeaponComponent rangeWeaponComponent = ref Weapons.Get(createdEnemyId);
        rangeWeaponComponent.MuzzleIds.Add(playerWeapon);

        ref OwningByComponent owningByComponent = ref _ownings.Get(playerWeapon);
        owningByComponent.Owner = createdEnemyId;

        ref var muzzle = ref Muzzles.Get(playerWeapon);
        muzzle.TimeBetweenShots = 3f;
    }
    
    protected virtual void InitializeEnemyAnimations(int createdEnemyId, EnemyInitializationInternalInfo enemyInitializationInternalInfo)
    {
        ref CharacterAnimationComponent enemyAnimations = ref _characterAnimations.Get(createdEnemyId);
        BaseStateMachine? animationStateMachine = Activator.CreateInstance(enemyInitializationInternalInfo.EnemyStateMachine, createdEnemyId, World) as BaseStateMachine;
        if (animationStateMachine is null)
        {
            throw new ArgumentException("Can't create animation state machine instance");
        }

        enemyAnimations.AnimationStateMachine = animationStateMachine;
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

    protected virtual void OnDeadEventHandler(int entityId)
    {
        ref CharacterAnimationComponent characterAnimationComponent = ref _characterAnimations.Get(entityId);
        if (characterAnimationComponent.Animations.TryGetValue(CharacterState.Death, out string animationId))
        {
            CreateDeathAnimationEntity(entityId, animationId);
        }
        else if(characterAnimationComponent.MultipleAnimations.TryGetValue(CharacterState.Death, out List<string> animationIds))
        {
            int index = Random.Shared.Next(animationIds.Count);
            CreateDeathAnimationEntity(entityId, animationIds[index]);
        }
    }

    protected virtual int CreateDeathAnimationEntity(int entityId, string deathAnimationId)
    {
        int createdAnimation = PrefabManager.CreateEntityFromPrefab(Builder, PrefabType.Animation);
            
        ref AnimationComponent animationComponent = ref _animations.Get(createdAnimation);
        ref AnimationComponent deadEnemyAnimationComponent = ref _animations.Get(entityId);

        animationComponent.AnimationId = deathAnimationId;
        animationComponent.CurrentFrameIndex = 0;
        animationComponent.ReflectByY = deadEnemyAnimationComponent.ReflectByY;

        ref SpriteComponent animationSpriteComponent = ref _sprites.Get(createdAnimation);
        ref SpriteComponent deadEnemySpriteComponent = ref _sprites.Get(entityId);
        animationSpriteComponent.LocalOffset = deadEnemySpriteComponent.LocalOffset;

        ref TransformComponent animationTransformComponent = ref _transforms.Get(createdAnimation);
        ref TransformComponent deadEnemyTransformComponent = ref _transforms.Get(entityId);
        animationTransformComponent.Location = deadEnemyTransformComponent.Location;
        animationTransformComponent.Scale = deadEnemyTransformComponent.Scale;

        return createdAnimation;
    }
}
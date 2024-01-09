using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations.StateMachines;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Damage;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Scene;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.AI.Factories;

public class NightBorneFactory : BaseEnemyFactory
{
    private EcsPool<DamageZoneComponent> _damageZones;
    public NightBorneFactory(
        EcsWorld world, 
        EntityBuilder builder, 
        ItemsFactory itemsFactory, 
        PrefabManager prefabManager
    ) : base(world, builder, itemsFactory, prefabManager)
    {
        _damageZones = world.GetPool<DamageZoneComponent>();
    }
    
    public override int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        EnemyInitializationInternalInfo initializationInternalInfo = new EnemyInitializationInternalInfo()
        {
            EnemyPrefab = PrefabType.NightBorne,
            EnemyWeaponPrefab = PrefabType.DefaultWeapon,
            EnemyBehaviorTree = typeof(NecromancerBt),
            EnemyStateMachine = typeof(IdleRunShootingHitDeathStateMachine)
        };
        
        int createdEntity = base.CreateInternal(enemyInitializationInfo, initializationInternalInfo);
        
        ref TransformComponent transformComponent = ref _transforms.Get(createdEntity);
        ref DynamicCollider dynamicCollider = ref _colliders.Get(createdEntity);
        dynamicCollider.Box = new Box(createdEntity, new Rectangle(
            Vector2.Zero, 
            dynamicCollider.DefaultSize * transformComponent.Scale
        ));

        return createdEntity;
    }

    protected override int CreateDeathAnimationEntity(int entityId, string deathAnimationId)
    {
        int animationEntityId = base.CreateDeathAnimationEntity(entityId, deathAnimationId);
        
        ref DynamicCollider dynamicCollider = ref _colliders.Has(animationEntityId)
            ? ref _colliders.Get(animationEntityId)
            : ref _colliders.Add(animationEntityId);

        ref TransformComponent animationTransform = ref _transforms.Get(animationEntityId);
        
        Vector2 boxSize = new Vector2(0.265f, 0.35f) * animationTransform.Scale;
        dynamicCollider.Box = new Box(animationEntityId, new Rectangle(
            animationTransform.Location - boxSize / 2, 
            boxSize
        ));
        dynamicCollider.Behavior = CollisionBehavior.Ignore;

        DamageZoneComponent damageZoneComponent = new DamageZoneComponent();
        Builder.AddTo(animationEntityId, damageZoneComponent);

        DamageComponent damageComponent = new DamageComponent
        {
            Damage = 50
        };
        Builder.AddTo(animationEntityId, damageComponent);

        return animationEntityId;
    }

    protected override void InitializeEnemyWeapon(int createdEnemyId, EnemyInitializationInternalInfo enemyInitializationInternalInfo)
    {
        int playerWeapon = PrefabManager.CreateEntityFromPrefab(Builder, enemyInitializationInternalInfo.EnemyWeaponPrefab);

        ref RangeWeaponComponent rangeWeaponComponent = ref Weapons.Get(createdEnemyId);
        rangeWeaponComponent.MuzzleIds.Add(playerWeapon);

        ref OwningByComponent owningByComponent = ref _ownings.Get(playerWeapon);
        owningByComponent.Owner = createdEnemyId;

        ref var muzzle = ref Muzzles.Get(playerWeapon);
        muzzle.TimeBetweenShots = 0.8f;
    }
}
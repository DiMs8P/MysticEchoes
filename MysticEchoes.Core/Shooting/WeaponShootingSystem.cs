﻿using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Scene;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class WeaponShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _systemExecutionContext;
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private EntityFactory _factory;

    private EcsFilter _weaponOwnersFilter;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<RangeWeaponComponent> _weapons;
    private EcsPool<MuzzleComponent> _muzzles;
    private EcsPool<MagicComponent> _magics;
    private EcsPool<OwningByComponent> _ownings;
    private EcsPool<DynamicCollider> _colliders;
    
    private EcsPool<BurstFireComponent> _bursts;
    private EcsPool<ChargeFireComponent> _charges;
    
    private EcsPool<ProjectileComponent> _projectiles;
    private EcsPool<HitscanComponent> _hitscans;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _weaponOwnersFilter = world.Filter<RangeWeaponComponent>().Inc<TransformComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _weapons = world.GetPool<RangeWeaponComponent>();
        _ownings = world.GetPool<OwningByComponent>();
        _colliders = world.GetPool<DynamicCollider>();
        _muzzles = world.GetPool<MuzzleComponent>();
        _magics = world.GetPool<MagicComponent>();

        _bursts = world.GetPool<BurstFireComponent>();
        _charges = world.GetPool<ChargeFireComponent>();

        _projectiles = world.GetPool<ProjectileComponent>();
        _hitscans = world.GetPool<HitscanComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var weaponOwnerId in _weaponOwnersFilter)
        {
            ref RangeWeaponComponent rangeWeaponComponent = ref _weapons.Get(weaponOwnerId);

            foreach (var weaponEntityId in rangeWeaponComponent.MuzzleIds)
            {
                ref MuzzleComponent muzzleComponent = ref _muzzles.Get(weaponEntityId);

                if (!muzzleComponent.CanFire)
                {
                    continue;
                }
                
                if (muzzleComponent.ShootingType is ShootingType.SingleShot)
                {
                    if (rangeWeaponComponent.IsShooting)
                    {
                        if (muzzleComponent.ElapsedTimeFromLastShot > muzzleComponent.TimeBetweenShots)
                        {
                            MakeShot(weaponOwnerId, weaponEntityId);
                            muzzleComponent.ElapsedTimeFromLastShot = 0;
                            continue;
                        }
                    }
                }
                
                if (muzzleComponent.ShootingType is ShootingType.BurstFire)
                {
                    ref BurstFireComponent burstFireComponent = ref _bursts.Get(weaponEntityId);

                    bool isShootingTime = (burstFireComponent.FiredShots != 0 && muzzleComponent.ElapsedTimeFromLastShot > burstFireComponent.TimeBetweenBurstShots) ||
                                          (burstFireComponent.FiredShots == 0 && rangeWeaponComponent.IsShooting && muzzleComponent.ElapsedTimeFromLastShot > muzzleComponent.TimeBetweenShots);

                    if (isShootingTime)
                    {
                        MakeShot(weaponOwnerId, weaponEntityId);
                        muzzleComponent.ElapsedTimeFromLastShot = 0;

                        bool reachedBurstLimit = burstFireComponent.FiredShots + 1 >= burstFireComponent.MaxShotsInBurst;
                        burstFireComponent.FiredShots = reachedBurstLimit ? 0 : burstFireComponent.FiredShots + 1;
                    }
                }

                if (muzzleComponent.ShootingType is ShootingType.Chargeable)
                {
                    ref ChargeFireComponent chargeFireComponent = ref _charges.Get(weaponEntityId);
    
                    if (rangeWeaponComponent.IsShooting)
                    {
                        chargeFireComponent.CurrentChargeTime += _systemExecutionContext.DeltaTime;
                    }

                    if ((rangeWeaponComponent.IsShooting && 
                         chargeFireComponent.CurrentChargeTime > chargeFireComponent.MaxChargeTime) ||
                        (!rangeWeaponComponent.IsShooting && chargeFireComponent.CurrentChargeTime > chargeFireComponent.MinChargeTime))
                    {
                        chargeFireComponent.CurrentChargeTime = 0;
                        MakeShot(weaponOwnerId, weaponEntityId);
                        muzzleComponent.ElapsedTimeFromLastShot = 0;
                        continue;
                    }

                    if (!rangeWeaponComponent.IsShooting)
                    {
                        chargeFireComponent.CurrentChargeTime = 0;
                    }
                }

                muzzleComponent.ElapsedTimeFromLastShot += _systemExecutionContext.DeltaTime;
            }
        }
    }

    private void MakeShot(int ownerEntityId, int weaponEntityId)
    {
        ref MuzzleComponent muzzleComponent = ref _muzzles.Get(weaponEntityId);
        ref TransformComponent weaponLocalTransformComponent = ref _transforms.Get(weaponEntityId);
        ref TransformComponent ownerTransformComponent = ref _transforms.Get(ownerEntityId);
        
        int magic = _prefabManager.CreateEntityFromPrefab(_factory, muzzleComponent.MagicPrefab);
        ref MagicComponent magicComponent = ref _magics.Get(magic);

        if (magicComponent.Type is MagicType.Projectile)
        {
            ref ProjectileComponent projectileComponent = ref _projectiles.Get(magic);
            
            InitializeProjectile(magic,
                weaponEntityId,
                ownerTransformComponent.Location + weaponLocalTransformComponent.Location,
                ownerTransformComponent.Rotation + weaponLocalTransformComponent.Rotation,
                projectileComponent.Size);
        }

        if (magicComponent.Type is MagicType.Hitscan)
        {
            ref HitscanComponent hitscanComponent = ref _hitscans.Get(weaponEntityId);
            
            /*SpawnHitscan(_factory,
                magicComponent.AmmoPrefab,
                ownerTransformComponent.Location + weaponLocalTransformComponent.Location,
                ownerTransformComponent.Rotation + weaponLocalTransformComponent.Rotation,
                weaponDamageComponent.Damage,
                0.2f);*/
        }
    }
    
    private void InitializeProjectile(int magicId, int weaponEntityId, Vector2 projectileLocation, Vector2 projectileRotation, float size)
    {
        ref OwningByComponent owningByComponent = ref _ownings.Get(weaponEntityId);
        owningByComponent.Owner = weaponEntityId;
        
        ref TransformComponent projectileTransformComponent = ref _transforms.Get(magicId);
        projectileTransformComponent.Location = projectileLocation;
        projectileTransformComponent.Rotation = projectileRotation;
        
        ref MovementComponent projectileMovementComponent = ref _movements.Get(magicId);
        projectileMovementComponent.Velocity = projectileRotation;

        ref DynamicCollider collider = ref _colliders.Get(magicId);
        collider.Box = new Box(magicId, new Rectangle(
            Vector2.Zero,
            Vector2.One * size * projectileTransformComponent.Scale  / 2
            ));
        collider.Behavior = CollisionBehavior.AllyBullet;
    }
}
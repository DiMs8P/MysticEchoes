using System.Numerics;
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
    private EcsPool<DamageComponent> _damages;
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
        _damages = world.GetPool<DamageComponent>();
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
                
                if (muzzleComponent.ShootingType == ShootingType.SingleShot)
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
                
                if (muzzleComponent.ShootingType == ShootingType.BurstFire)
                {
                    ref BurstFireComponent burstFireComponent = ref _bursts.Get(weaponEntityId);

                    bool isShootingTime = (burstFireComponent.FiredShots != 0 && muzzleComponent.ElapsedTimeFromLastShot > burstFireComponent.TimeBetweenBurstShots) ||
                                          (burstFireComponent.FiredShots == 0 && rangeWeaponComponent.IsShooting && muzzleComponent.ElapsedTimeFromLastShot > muzzleComponent.TimeBetweenShots);
    
                    if (!isShootingTime)
                        return;

                    MakeShot(weaponOwnerId, weaponEntityId);
                    muzzleComponent.ElapsedTimeFromLastShot = 0;

                    bool reachedBurstLimit = burstFireComponent.FiredShots + 1 >= burstFireComponent.MaxShotsInBurst;
                    burstFireComponent.FiredShots = reachedBurstLimit ? 0 : burstFireComponent.FiredShots + 1;

                    continue;
                }

                if (muzzleComponent.ShootingType == ShootingType.Chargeable)
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
        ref MagicComponent magicComponent = ref _magics.Get(weaponEntityId);
        ref TransformComponent weaponLocalTransformComponent = ref _transforms.Get(weaponEntityId);
        ref TransformComponent ownerTransformComponent = ref _transforms.Get(ownerEntityId);
        
        ref DamageComponent weaponDamageComponent = ref _damages.Get(weaponEntityId);

        if (magicComponent.Type == MagicType.Projectile)
        {
            ref ProjectileComponent projectileComponent = ref _projectiles.Get(weaponEntityId);
            
            SpawnProjectile(_factory,
                magicComponent.MagicPrefab,
                ownerTransformComponent.Location + weaponLocalTransformComponent.Location,
                ownerTransformComponent.Rotation + weaponLocalTransformComponent.Rotation,
                weaponDamageComponent.Damage,
                projectileComponent.Speed);
        }

        if (magicComponent.Type == MagicType.Hitscan)
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
    
    private int SpawnProjectile(EntityFactory factory, PrefabType projectileId, Vector2 projectileLocation, Vector2 projectileRotation, float damage, float speed)
    {
        int projectile = _prefabManager.CreateEntityFromPrefab(factory, projectileId);

        ref TransformComponent projectileTransformComponent = ref _transforms.Get(projectile);
        projectileTransformComponent.Location = projectileLocation;
        projectileTransformComponent.Rotation = projectileRotation;
        
        ref MovementComponent projectileMovementComponent = ref _movements.Get(projectile);
        projectileMovementComponent.Velocity = projectileRotation;
        projectileMovementComponent.Speed = speed;
        
        ref DamageComponent projectileDamageComponent = ref _damages.Get(projectile);
        projectileDamageComponent.Damage = damage;

        ref DynamicCollider collider = ref _colliders.Get(projectile);
        collider.Box = new Box(projectile, new Rectangle(
            Vector2.Zero,
            Vector2.One * 0.5f * projectileTransformComponent.Scale  / 2
            ));
        collider.Behavior = CollisionBehavior.AllyBullet;

        return projectile;
    }
}
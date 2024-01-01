using System.Numerics;
using Leopotam.EcsLite;
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
    private EcsPool<WeaponOwnerComponent> _weapons;
    private EcsPool<MuzzleComponent> _muzzles;
    private EcsPool<MagicComponent> _magics;
    private EcsPool<DamageComponent> _damages;
    private EcsPool<BurstFireComponent> _bursts;
    
    private EcsPool<ProjectileComponent> _projectiles;
    private EcsPool<HitscanComponent> _hitscans;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _weaponOwnersFilter = world.Filter<WeaponOwnerComponent>().Inc<TransformComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _weapons = world.GetPool<WeaponOwnerComponent>();
        _damages = world.GetPool<DamageComponent>();
        _muzzles = world.GetPool<MuzzleComponent>();
        _magics = world.GetPool<MagicComponent>();

        _bursts = world.GetPool<BurstFireComponent>();

        _projectiles = world.GetPool<ProjectileComponent>();
        _hitscans = world.GetPool<HitscanComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var weaponOwnerId in _weaponOwnersFilter)
        {
            ref WeaponOwnerComponent weaponOwnerComponent = ref _weapons.Get(weaponOwnerId);

            foreach (var weaponEntityId in weaponOwnerComponent.WeaponIds)
            {
                ref MuzzleComponent muzzleComponent = ref _muzzles.Get(weaponEntityId);

                if (!muzzleComponent.CanFire)
                {
                    continue;
                }
                
                if (muzzleComponent.ShootingType == ShootingType.SingleShot)
                {
                    if (weaponOwnerComponent.IsShooting)
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

                    if (burstFireComponent.FiredShots != 0)
                    {
                        if (muzzleComponent.ElapsedTimeFromLastShot > burstFireComponent.TimeBetweenBurstShots)
                        {
                            MakeShot(weaponOwnerId, weaponEntityId);
                            muzzleComponent.ElapsedTimeFromLastShot = 0;
                            
                            if (burstFireComponent.FiredShots + 1 >= burstFireComponent.MaxShotsInBurst)
                            {
                                burstFireComponent.FiredShots = 0;
                            }
                            else
                            {
                                ++burstFireComponent.FiredShots;
                            }
                            
                            continue;
                        }
                    }
                    else
                    {
                        if (weaponOwnerComponent.IsShooting)
                        {
                            if (muzzleComponent.ElapsedTimeFromLastShot > muzzleComponent.TimeBetweenShots)
                            {
                                MakeShot(weaponOwnerId, weaponEntityId);
                                muzzleComponent.ElapsedTimeFromLastShot = 0;
                                ++burstFireComponent.FiredShots;
                                continue;
                            }
                        }
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

        if (magicComponent.Type == AmmoType.Projectile)
        {
            ref ProjectileComponent projectileComponent = ref _projectiles.Get(weaponEntityId);
            
            SpawnProjectile(_factory,
                magicComponent.AmmoPrefab,
                ownerTransformComponent.Location + weaponLocalTransformComponent.Location,
                ownerTransformComponent.Rotation + weaponLocalTransformComponent.Rotation,
                weaponDamageComponent.Damage,
                projectileComponent.Speed);
        }

        if (magicComponent.Type == AmmoType.Hitscan)
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
        
        return projectile;
    }
}
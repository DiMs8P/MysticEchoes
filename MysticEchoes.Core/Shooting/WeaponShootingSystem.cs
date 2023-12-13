using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class WeaponShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private EntityFactory _factory;
    [EcsInject] private SystemExecutionContext _systemExecutionContext;

    private EcsFilter _weaponsFilter;
    private EcsPool<WeaponComponent> _weapons;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<ShootRequest> _shootRequests;
    private WeaponsSettings _weaponsSettings;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _weaponsFilter = world.Filter<WeaponComponent>().Inc<TransformComponent>().End();

        _weapons = world.GetPool<WeaponComponent>();
        _transforms = world.GetPool<TransformComponent>();
        _shootRequests = world.GetPool<ShootRequest>();
        _weaponsSettings = _systemExecutionContext.Settings.WeaponsSettings;
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _weaponsFilter)
        {
            ref WeaponComponent weaponComponent = ref _weapons.Get(entityId);

            if (_shootRequests.Has(entityId))
            {
                _shootRequests.Del(entityId);

                if (weaponComponent.ElapsedTimeFromLastShoot >= weaponComponent.TimeBetweenShoots) // нужно что-то придумать с временем между выстрелами, оно у каждого оружия разное и размер пули
                {
                    MakeShot(entityId);
                    weaponComponent.ElapsedTimeFromLastShoot = 0;
                    continue;
                }
            }

            weaponComponent.ElapsedTimeFromLastShoot += _systemExecutionContext.DeltaTime;
        }
    }

    private void MakeShot(int entityId)
    {
        ref WeaponComponent weapon = ref _weapons.Get(entityId);

        if (weapon.Type is WeaponType.OneShoot)
        {
            ref TransformComponent transformComponent = ref _transforms.Get(entityId);

            SpawnProjectile(_factory,
                transformComponent.Location,
                transformComponent.Rotation,
                _weaponsSettings.OneShot.Damage,
                _weaponsSettings.OneShot.BulletSpeed);
        }
        else if (weapon.Type is WeaponType.TwoShoot)
        {
            ref TransformComponent transformComponent = ref _transforms.Get(entityId);

            SpawnProjectile(_factory,
                transformComponent.Location + transformComponent.Rotation.Inverse().ReflectY() * _weaponsSettings.Twoshot.DistanceBetweenBullets / 2,
                transformComponent.Rotation,
                _weaponsSettings.Twoshot.Damage,
                _weaponsSettings.Twoshot.BulletSpeed);
            SpawnProjectile(_factory,
                transformComponent.Location + transformComponent.Rotation.Inverse().ReflectY() * _weaponsSettings.Twoshot.DistanceBetweenBullets / 2 * (-1),
                transformComponent.Rotation,
                _weaponsSettings.Twoshot.Damage,
                _weaponsSettings.Twoshot.BulletSpeed);
        }
        else if (weapon.Type is not WeaponType.None)
        {
            throw new NotImplementedException();
        }
    }

    private int SpawnProjectile(EntityFactory factory, Vector2 projectileLocation, Vector2 projectileRotation, float damage, float speed)
    {
        int projectile = factory.Create()
            .Add(new TransformComponent()
            {
                Location = projectileLocation,
                Rotation = projectileRotation
            })
            .Add(new MovementComponent()
            {
                Velocity = projectileRotation,
                Speed = speed
            })
            .Add(new DamageComponent()
            {
                Damage = damage
            })
            .Add(new RenderComponent(RenderingType.Bullet))
            .End();

        return projectile;
    }
}
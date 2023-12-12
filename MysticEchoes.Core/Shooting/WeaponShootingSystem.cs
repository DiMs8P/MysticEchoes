using System.Numerics;
using Leopotam.EcsLite;
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
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _weaponsFilter = world.Filter<WeaponComponent>().Inc<TransformComponent>().End();

        _weapons = world.GetPool<WeaponComponent>();
        _transforms = world.GetPool<TransformComponent>();
        _shootRequests = world.GetPool<ShootRequest>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _weaponsFilter)
        {
            ref WeaponComponent weaponComponent = ref _weapons.Get(entityId);

            if (_shootRequests.Has(entityId))
            {
                _shootRequests.Del(entityId);

                if (weaponComponent.ElapsedTimeFromLastShoot >= weaponComponent.TimeBetweenShoots)
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

            SpawnProjectile(_factory, transformComponent.Location, transformComponent.Rotation, 10.0f, 1.0f);
        }
        else if (weapon.Type is WeaponType.TwoShoot)
        {
            ref TransformComponent transformComponent = ref _transforms.Get(entityId);

            SpawnProjectile(_factory, transformComponent.Location + transformComponent.Rotation.Inverse().Lover() * 0.04f, transformComponent.Rotation, 10.0f, 0.2f);
            SpawnProjectile(_factory, transformComponent.Location + transformComponent.Rotation.Inverse().Lover() * -0.04f, transformComponent.Rotation, 10.0f, 0.2f);
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
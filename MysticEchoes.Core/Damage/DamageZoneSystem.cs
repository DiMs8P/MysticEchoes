using Leopotam.EcsLite;
using MysticEchoes.Core.Health;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.Damage;

public class DamageZoneSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _damageZoneFilter;
    
    private EcsPool<DamageZoneComponent> _damageZones;
    private EcsPool<DamageComponent> _damages;
    private EcsPool<HealthComponent> _health;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _damageZoneFilter = world.Filter<DamageZoneComponent>().End();
        
        _damageZones = world.GetPool<DamageZoneComponent>();
        _damages = world.GetPool<DamageComponent>();
        _health = world.GetPool<HealthComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var damageZoneId in _damageZoneFilter)
        {
            ref DamageZoneComponent damageZoneComponent = ref _damageZones.Get(damageZoneId);

            foreach (var entityId in damageZoneComponent.EntityToDamage)
            {
                if (damageZoneComponent.DamagedEntities.Contains(entityId))
                {
                    continue;
                }
                
                ref HealthComponent healthComponent = ref _health.Get(entityId);
                ref DamageComponent damageComponent = ref _damages.Get(damageZoneId);

                if (!healthComponent.Immortal)
                {
                    healthComponent.Health -= damageComponent.Damage;
                }

                damageZoneComponent.EntityToDamage.Remove(entityId);
                damageZoneComponent.DamagedEntities.Add(entityId);
            }
        }
    }
}
using Leopotam.EcsLite;

namespace MysticEchoes.Core.Health;

public class HealthSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorld _world;
    
    private EcsFilter _healthFilter;
    private EcsPool<HealthComponent> _health;
    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _healthFilter = _world.Filter<HealthComponent>().End();
        _health = _world.GetPool<HealthComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var healthId in _healthFilter)
        {
            ref HealthComponent entityHealth = ref _health.Get(healthId);

            if (entityHealth is { Health: <= 0.0f, Immortal: false })
            {
                _world.DelEntity(healthId);
            }
        }
    }
}
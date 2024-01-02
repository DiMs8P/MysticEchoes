using Leopotam.EcsLite;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class LifeTimeCleanupSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _context;
    
    private EcsWorld _world;
    
    private EcsFilter _lifeTimeFilter;
    private EcsPool<LifeTimeComponent> _lifeTimes;
    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _lifeTimeFilter = _world.Filter<LifeTimeComponent>().End();
        _lifeTimes = _world.GetPool<LifeTimeComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (int entityId in _lifeTimeFilter)
        {
            ref LifeTimeComponent lifeTimeComponent = ref _lifeTimes.Get(entityId);

            if (!lifeTimeComponent.IsActive)
            {
                continue;
            }

            lifeTimeComponent.LifeTime -= _context.DeltaTime;

            if (lifeTimeComponent.LifeTime <= 0)
            {
                _world.DelEntity(entityId);
            }
        }
    }
}
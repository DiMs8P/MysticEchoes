using Leopotam.EcsLite;

namespace MysticEchoes.Core.AI;

public class AiSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _aiFilter;
    private EcsPool<AiComponent> _ais; 
    
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _aiFilter = world.Filter<AiComponent>().End();
        _ais = world.GetPool<AiComponent>();
        
        foreach (var enemyId in _aiFilter)
        {
            ref AiComponent aiComponent = ref _ais.Get(enemyId);
            aiComponent.BehaviorTree.Start();
        }
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var enemyId in _aiFilter)
        {
            ref AiComponent aiComponent = ref _ais.Get(enemyId);
            aiComponent.BehaviorTree.Update();
        }
    }
}
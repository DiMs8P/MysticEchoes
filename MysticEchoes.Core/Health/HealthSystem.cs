using Leopotam.EcsLite;
using MysticEchoes.Core.AI;
using MysticEchoes.Core.Events;
using MysticEchoes.Core.Player;
using MysticEchoes.Core.Scene;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Health;

public class HealthSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private GameplayEventListener _eventListener;
    
    private EcsWorld _world;
    
    private EcsFilter _healthFilter;
    private EcsPool<HealthComponent> _health;
    private EcsPool<EnemyComponent> _enemies;
    private EcsPool<PlayerMarker> _player;
    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _healthFilter = _world.Filter<HealthComponent>().End();
        _health = _world.GetPool<HealthComponent>();

        _enemies = _world.GetPool<EnemyComponent>();
        _player = _world.GetPool<PlayerMarker>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var healthId in _healthFilter)
        {
            ref HealthComponent entityHealth = ref _health.Get(healthId);

            if (entityHealth is { Health: <= 0.0f, Immortal: false })
            {
                entityHealth.InvokeOnPreDead(healthId);
                ProcessPlayerOrEnemyDeath(healthId);
                _world.DelEntity(healthId);
            }
        }
    }

    private void ProcessPlayerOrEnemyDeath(int healthId)
    {
        if (_enemies.Has(healthId))
        {
            ref EnemyComponent enemyComponent = ref _enemies.Get(healthId);
            
            OnEnemyDeadInfo enemyDeadInfo = new OnEnemyDeadInfo();
            enemyDeadInfo.EnemyId = enemyComponent.EnemyId;
            enemyDeadInfo.EntityId = healthId;
            enemyDeadInfo.RoomId = enemyComponent.RoomId;
            _eventListener.InvokeOnEnemyDead(enemyDeadInfo);
        }
        else if (_player.Has(healthId))
        {
            OnPlayerDeadInfo playerDeadInfo = new OnPlayerDeadInfo();

            _eventListener.InvokeOnPlayerDead(playerDeadInfo);
        }
    }
}
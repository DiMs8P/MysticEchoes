using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class EnemyFactory
{
    private Dictionary<int, IEnemyFactory> _factories = new Dictionary<int, IEnemyFactory>();

    public EnemyFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager)
    {
        _factories.Add(0, new NecromancerFactory(world, builder, prefabManager));
    }

    public int CreateEnemy(EnemyInitializationInfo enemyInitializationInfo)
    {
        if (_factories.TryGetValue(enemyInitializationInfo.EnemyId, out IEnemyFactory enemyFactory))
        {
            return enemyFactory.Create(enemyInitializationInfo);
        }
        
        throw new ArgumentException($"Invalid enemy Id: {enemyInitializationInfo.EnemyId}");
    }
}
using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class EnemyFactory
{
    private Dictionary<int, IEnemyFactory> _factories = new Dictionary<int, IEnemyFactory>();

    public EnemyFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager, EnemySettings enemySettings)
    {
        _factories.Add(0, new NecromancerFactory(world, builder, prefabManager, enemySettings.Enemies[0]));
    }

    public int CreateEnemy(int enemyId)
    {
        if (_factories.TryGetValue(enemyId, out IEnemyFactory enemyFactory))
        {
            return enemyFactory.Create();
        }
        
        throw new ArgumentException($"Invalid enemy Id: {enemyId}");
    }
}
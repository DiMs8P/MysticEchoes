using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class EnemyFactory
{
    private Dictionary<int, IEnemyFactory> _factories = new Dictionary<int, IEnemyFactory>();

    public EnemyFactory(EcsWorld world, EntityBuilder builder, ItemsFactory itemsFactory, PrefabManager prefabManager)
    {
        _factories.Add(0, new NecromancerFactory(world, builder, itemsFactory, prefabManager));
        _factories.Add(1, new BringerFactory(world, builder, itemsFactory, prefabManager));
        _factories.Add(2, new NightBorneFactory(world, builder, itemsFactory, prefabManager));
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
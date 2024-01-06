using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class BaseEnemyFactory : IEnemyFactory
{
    public BaseEnemyFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager, EnemyInfo enemyInfo)
    {
        
    }

    public virtual int Create()
    {
        throw new NotImplementedException();
    }
}
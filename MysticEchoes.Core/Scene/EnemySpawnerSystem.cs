using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI;
using MysticEchoes.Core.AI.Factories;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Movement;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class EnemySpawnerSystem : IEcsInitSystem
{
    [EcsInject] private EnemyFactory _enemyFactory;
    public void Init(IEcsSystems systems)
    {
        CreateEnemy();
    }
    
    private void CreateEnemy()
    {
        EnemyInitializationInfo enemyInitializationInfo = new EnemyInitializationInfo();
        enemyInitializationInfo.EnemyId = 0;
        enemyInitializationInfo.Location = new Vector2(1.0f, 1.0f);
        
        _enemyFactory.CreateEnemy(enemyInitializationInfo);
    }
}
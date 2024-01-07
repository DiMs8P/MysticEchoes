using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.AI;

public class EnemyInitializationInternalInfo
{
    public required PrefabType EnemyPrefab;
    public required PrefabType EnemyWeaponPrefab;

    public required Type EnemyBehaviorTree;
    public required Type EnemyStateMachine;
}
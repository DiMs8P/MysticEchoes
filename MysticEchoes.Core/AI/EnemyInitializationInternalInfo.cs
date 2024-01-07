using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.AI;

public class EnemyInitializationInternalInfo
{
    public PrefabType EnemyPrefab;
    public PrefabType EnemyWeaponPrefab;

    public Type EnemyBehaviorTree;
    public Type EnemyStateMachine;
}
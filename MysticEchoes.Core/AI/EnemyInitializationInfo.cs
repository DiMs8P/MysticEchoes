using System.Numerics;
using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.AI;

public class EnemyInitializationInfo
{
    public int EnemyId;
    public PrefabType EnemyPrefab;
    public PrefabType EnemyWeaponPrefab;
    public Vector2 Location;
}
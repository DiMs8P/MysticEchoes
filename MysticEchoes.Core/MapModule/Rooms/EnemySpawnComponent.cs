using System.Numerics;
using MazeGeneration.Enemies;
using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.MapModule.Rooms;

public struct EnemySpawnComponent
{
    public Rectangle Area { get; set; }
    public EnemyType Type { get; set; }
}
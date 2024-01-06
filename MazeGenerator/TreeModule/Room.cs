using System.Drawing;
using MazeGeneration.Enemies;

namespace MazeGeneration.TreeModule;

public class Room
{
    public Rectangle Shape { get; set; }
    public HashSet<Point> Doors { get; set; } = new();
    public List<EnemySpawn> EnemySpawns { get; set; } = new();
    public int ValueCost { get; set; }
    public RoomType Type { get; set; }
}

public enum RoomType
{
    PlayerSpawn,
    Battle
}
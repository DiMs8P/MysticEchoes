using System.Drawing;
using MazeGeneration.Enemies;

namespace MazeGeneration.TreeModule.Rooms;

public class Room
{
    public Rectangle Shape { get; set; }
    public HashSet<Door> Doors { get; set; } = new();
    public List<EnemySpawn> EnemySpawns { get; set; } = new();
    public int ValueCost { get; set; }
    public RoomType Type { get; set; }
}


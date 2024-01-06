using System.Drawing;

namespace MazeGeneration.Enemies;

public class EnemySpawn
{
    public Rectangle Area { get; set; }
    public HashSet<Point> Tiles { get; set; } = new();
    public EnemyType Type { get; set; }
}
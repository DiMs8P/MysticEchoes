using System.Drawing;
using System.Numerics;

namespace MazeGeneration;

public class Maze
{
    public CellType[,] Cells { get; }
    public Size Size { get; }
    public HashSet<Point> Floor { get; } = new();
    public HashSet<Point> Walls { get; } = new();

    public Maze(Size size)
    {
        Size = size;
        Cells = new CellType[size.Height, size.Width];
    }
}

public enum CellType
{
    Empty,
    FragmentBound,
    Hall,
    Wall,
    ControlPoint,
    Floor
}
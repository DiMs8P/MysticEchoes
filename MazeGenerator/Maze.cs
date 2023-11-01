using System.Drawing;

namespace MazeGeneration;

public class Maze
{
    public CellType[,] Cells { get; }
    public Size Size { get; }

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
    ControlPoint
}
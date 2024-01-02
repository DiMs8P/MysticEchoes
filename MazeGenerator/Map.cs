using MazeGeneration.TreeModule;
using System.Drawing;

namespace MazeGeneration;

public class Map
{
    public Size Size { get; }
    public HashSet<Point> FloorTiles { get; } = new();
    public HashSet<Point> DoorTiles { get; set; } = new();
    public HashSet<Point> WallTiles { get; } = new();
    public Tree<RoomNode> BinarySpaceTree { get; }

    public Map(Size size, Tree<RoomNode> tree)
    {
        Size = size;
        BinarySpaceTree = tree;
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
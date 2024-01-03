using MazeGeneration.TreeModule;
using System.Drawing;
using MazeGeneration.Walls;

namespace MazeGeneration;

public class Map
{
    public Size Size { get; }
    public HashSet<Point> FloorTiles { get; } = new();
    public HashSet<Point> DoorTiles { get; set; } = new();
    public HashSet<Point> WallTopTiles { get; } = new();
    public HashSet<Point> WallSieRightTiles { get; } = new();
    public HashSet<Point> WallSideLeftTiles { get; } = new();
    public HashSet<Point> WallBottomTiles { get; } = new();
    public HashSet<Point> WallFullTiles { get; } = new();

    public Tree<RoomNode> BinarySpaceTree { get; }

    public Map(Size size, Tree<RoomNode> tree)
    {
        Size = size;
        BinarySpaceTree = tree;
    }

    internal void AddSingleCornerWall(Point position, string neighborsBinary)
    {

    }

    internal void AddSingleBasicWall(Point position, string neighborsBinary)
    {
        int type = Convert.ToInt32(neighborsBinary, 2);

        HashSet<Point> tileCollection = null;

        if (WallTypesHelper.WallTop.Contains(type))
        {
            tileCollection = WallTopTiles;
        }

        if (tileCollection is not null)
        {
            tileCollection.Add(position);
        }

        //WallTopTiles.Add(position);
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
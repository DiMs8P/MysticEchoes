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
    public HashSet<Point> WallSideRightTiles { get; } = new();
    public HashSet<Point> WallSideLeftTiles { get; } = new();
    public HashSet<Point> WallBottomTiles { get; } = new();
    public HashSet<Point> WallFullTiles { get; } = new();

    public HashSet<Point> WallInnerCornerDownLeft { get; } = new();
    public HashSet<Point> WallInnerCornerDownRight { get; } = new();

    public HashSet<Point> WallDiagonalCornerDownRight { get; } = new();
    public HashSet<Point> WallDiagonalCornerDownLeft { get; } = new();
    public HashSet<Point> WallDiagonalCornerUpRight { get; } = new();
    public HashSet<Point> WallDiagonalCornerUpLeft { get; } = new();

    public Point PlayerSpawn { get; set; }

    public Tree<RoomNode> BinarySpaceTree { get; }

    public Map(Size size, Tree<RoomNode> tree)
    {
        Size = size;
        BinarySpaceTree = tree;
    }

    internal void AddSingleCornerWall(Point position, string neighborsBinary)
    {
        int type = Convert.ToInt32(neighborsBinary, 2);

        HashSet<Point> tileCollection = null;

        if (WallTypesHelper.WallInnerCornerDownLeft.Contains(type))
        {
            tileCollection = WallInnerCornerDownLeft;
        }
        else if(WallTypesHelper.WallInnerCornerDownRight.Contains(type))
        {
            tileCollection = WallInnerCornerDownRight;
        }
        else if (WallTypesHelper.WallDiagonalCornerDownLeft.Contains(type))
        {
            tileCollection = WallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.WallDiagonalCornerDownRight.Contains(type))
        {
            tileCollection = WallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.WallDiagonalCornerUpRight.Contains(type))
        {
            tileCollection = WallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.WallDiagonalCornerUpLeft.Contains(type))
        {
            tileCollection = WallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.WallFullEightDirections.Contains(type))
        {
            tileCollection = WallFullTiles;
        }
        else if (WallTypesHelper.WallBottomEightDirections.Contains(type))
        {
            tileCollection = WallBottomTiles;
        }

        if (tileCollection is not null)
        {
            tileCollection.Add(position);
        }
    }

    internal void AddSingleBasicWall(Point position, string neighborsBinary)
    {
        int type = Convert.ToInt32(neighborsBinary, 2);

        HashSet<Point> tileCollection = null;

        if (WallTypesHelper.WallTop.Contains(type))
        {
            tileCollection = WallTopTiles;
        } 
        else if (WallTypesHelper.WallSideRight.Contains(type))
        {
            tileCollection = WallSideRightTiles;
        }
        else if (WallTypesHelper.WallSideLeft.Contains(type))
        {
            tileCollection = WallSideLeftTiles;
        }
        else if (WallTypesHelper.WallBottom.Contains(type))
        {
            tileCollection = WallBottomTiles;
        }
        else if (WallTypesHelper.WallFull.Contains(type))
        {
            tileCollection = WallFullTiles;
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
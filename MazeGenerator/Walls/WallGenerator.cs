using System.Drawing;

namespace MazeGeneration.Walls;

public class WallGenerator
{
    public void CreateWalls(Map map)
    {
        var basicWallPositions = FindWallsInDirections(map.FloorTiles, Directions2D.CardinalDirections);
        var cornerWallPositions = FindWallsInDirections(map.FloorTiles, Directions2D.DiagonalDirections);

        CreateBasicWalls(map, basicWallPositions, map.FloorTiles);
        CreateCornerWalls(map, cornerWallPositions, map.FloorTiles);
    }

    private void CreateCornerWalls(Map map, HashSet<Point> cornerWallPositions, HashSet<Point> floorTiles)
    {
        foreach (var position in cornerWallPositions)
        {
            var neighborsBinary = String.Empty;
            foreach (var direction in Directions2D.EightDirections)
            {
                var neighbor = new Point(position.X + direction.X, position.Y + direction.Y);
                if (floorTiles.Contains(neighbor))
                {
                    neighborsBinary += "1";
                }
                else
                {
                    neighborsBinary += "0";
                }
            }

            map.AddSingleCornerWall(position, neighborsBinary);
        }
    }

    private static void CreateBasicWalls(Map map, HashSet<Point> basicWallPositions, HashSet<Point> floorTiles)
    {
        foreach (var position in basicWallPositions)
        {
            var neighborsBinary = String.Empty;
            foreach (var direction in Directions2D.CardinalDirections)
            {
                var neighbor = new Point(position.X + direction.X, position.Y + direction.Y);
                if (floorTiles.Contains(neighbor))
                {
                    neighborsBinary += "1";
                }
                else
                {
                    neighborsBinary += "0";
                }
            }
            map.AddSingleBasicWall(position, neighborsBinary);
        }
    }

    private HashSet<Point> FindWallsInDirections(HashSet<Point> floorTiles, Point[] directions)
    {
        return floorTiles
            .SelectMany(
                _ => directions,
                (floor, direction) => new Point(floor.X + direction.X, floor.Y + direction.Y))
            .Where(neighborPosition => !floorTiles.Contains(neighborPosition))
            .ToHashSet();
    }
}

public class Directions2D
{
    public static readonly Point[] CardinalDirections =
    {
        new(0, 1),
        new(1, 0),
        new(0, -1),
        new(-1, 0)
    };
    public static readonly Point[] DiagonalDirections =
    {
        new(1, 1),
        new(1, -1),
        new(-1, -1),
        new(-1, 1)
    };
    public static readonly Point[] EightDirections =
    {
        new(0, 1),
        new(1, 1),
        new(1, 0),
        new(1, -1),
        new(0, -1),
        new(-1, -1),
        new(-1, 0),
        new(-1, 1)
    };
}
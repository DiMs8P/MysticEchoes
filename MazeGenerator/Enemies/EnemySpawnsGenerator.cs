using System;
using System.Drawing;
using MazeGeneration.TreeModule;

namespace MazeGeneration.Enemies;

public class EnemySpawnsGenerator
{
    private readonly GenerationRandom _random;
    private readonly EnemySpawnsGeneratorParameter _spawnsGeneratorParameter;

    public EnemySpawnsGenerator(GenerationRandom random, EnemySpawnsGeneratorParameter spawnsGeneratorParameter)
    {
        _random = random;
        _spawnsGeneratorParameter = spawnsGeneratorParameter;
    }

    public void Generate(Map map, Tree<RoomNode> roomsTree)
    {
        var rooms = roomsTree.DeepCrawl()
            .Where(x => x.Room is not null)
            .Select(x => x.Room)
            .ToArray();

        foreach (var room in rooms)
        {
            var enemies = new List<EnemyType>();

            var roomTiles = map.FloorTiles
                        .Where(floor => room!.Shape.ContainsNotStrict(floor))
                        .ToHashSet();
            var remainingCost = roomTiles.Count;

            do
            {
                if (!_spawnsGeneratorParameter.NextEnemy(remainingCost, out var enemy))
                {
                    break;
                }
                remainingCost -= _spawnsGeneratorParameter.GetCost(enemy);
                enemies.Add(enemy);
            } while (remainingCost > 0);

            PlaceEnemiesIntoRoom(room!, enemies, roomTiles);
        }
    }

    private void PlaceEnemiesIntoRoom(Room room, List<EnemyType> enemies, HashSet<Point> roomTiles)
    {
        var availableTiles = roomTiles.ToHashSet();

        enemies = enemies.OrderByDescending(x =>
            {
                var size = _spawnsGeneratorParameter.GetSize(x);
                return int.Max(size.X, size.Y);
            })
            .ToList();
        var enemiesForRemove = new List<EnemyType>();

        foreach (var enemyType in enemies)
        {
            var size = _spawnsGeneratorParameter.GetSize(enemyType);
            if (!FindPlace(availableTiles, size, out var place))
            {
                continue;
            }

            var coveredTiles = GetCoveredTiles(place, size);
            foreach (var coveredTile in coveredTiles)
            {
                availableTiles.Remove(coveredTile);
            }
            enemiesForRemove.Add(enemyType);

            room.EnemySpawns.Add(new EnemySpawn
            {
                Area = new Rectangle(place, new Size(size)),
                Tiles = coveredTiles,
                Type = enemyType
            });
        }
    }

    private bool FindPlace(HashSet<Point> tiles, Point size, out Point foundedPlace)
    {
        foreach (var tile in 
                 from tile in tiles 
                 let coveredTiles = GetCoveredTiles(tile, size) 
                 where coveredTiles.All(tiles.Contains) 
                 select tile)
        {
            foundedPlace = tile;
            return true;
        }

        foundedPlace = Point.Empty;
        return false;
    }

    private HashSet<Point> GetCoveredTiles(Point tile, Point size)
    {
        var rectangle = new Rectangle(tile, new Size(size));

        var result = new HashSet<Point>();
        for (var x = rectangle.Left; x <= rectangle.Right; x++)
        {
            for (int y = rectangle.Top; y <= rectangle.Bottom; y++)
            {
                result.Add(new Point(x, y));
            }
        }

        return result;
    }
}
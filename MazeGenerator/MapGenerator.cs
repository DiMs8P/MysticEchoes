﻿using MazeGeneration.TreeModule;
using System.Drawing;
using MazeGeneration.Walls;
using MazeGeneration.Enemies;
using MazeGeneration.TreeModule.Rooms;

namespace MazeGeneration;

public class MapGenerator
{
    private readonly GenerationConfig _config;

    public MapGenerator(GenerationConfig config)
    {
        _config = config;
    }

    public Map Generate()
    {
        var tree = new Tree<RoomNode>(new RoomNode(Point.Empty, _config.MazeSize));
        tree.Root.Depth = 1;

        var map = new Map(_config.MazeSize, tree);

        // Design
        MakeLeafs(tree);
        MakeRooms(tree);
        MakeHalls(tree);
        //NormalizeHalls(tree);
        RemoveExtraControlPoints(tree);
        MakeDoors(tree);

        // Tile-level
        MarkFloor(tree, map);
        MakeWalls(map);
        MakeDoors(tree, map);

        // High-level
        MarkupRooms(tree, map);
        MakePlayerSpawn(tree, map);
        MakeEnemySpawns(tree, map);


        return map;
    }

    private void MakePlayerSpawn(Tree<RoomNode> tree, Map map)
    {
        var playerSpawnRoom = tree.DeepCrawl()
            .First(x => x.Room?.Type is RoomType.PlayerSpawn)
            .Room!;

        var center = playerSpawnRoom.Shape.GetCenter();
        map.PlayerSpawn = center;
    }

    private void MarkupRooms(Tree<RoomNode> tree, Map map)
    {
        foreach (var room in tree.DeepCrawl()
                     .Where(x => x.Room is not null)
                     .Select(x => x.Room))
        {
            var roomTiles = map.FloorTiles
                .Where(floor => room!.Shape.ContainsNotStrict(floor))
                .ToHashSet();
            room!.ValueCost = roomTiles.Count;
            room.Type = RoomType.Battle;
        }

        var playerSpawnRoom = tree.DeepCrawl()
            .Where(x => x.Room is not null)
            .MinBy(x => x.Room!.ValueCost);

        playerSpawnRoom.Room.Type = RoomType.PlayerSpawn;
    }

    private void MakeEnemySpawns(Tree<RoomNode> tree, Map map)
    {
        var enemySpawnsGenerator = new EnemySpawnsGenerator(
            _config.Random, 
            _config.EnemySpawnsGenerator
        );

        enemySpawnsGenerator.Generate(map, tree);
    }

    private void MakeDoors(Tree<RoomNode> tree)
    {
        Point GetEdgePoint(Point direction, Rectangle room, Point innerPoint)
        {
            if (direction.Y == 0)
            {
                var edgeX = direction.X > 0
                    ? room.Right
                    : room.Left;

                return innerPoint with { X = edgeX };
            }
            var edgeY = direction.Y > 0
                ? room.Y + room.Height
                : room.Y;

            return innerPoint with { Y = edgeY };
        }

        DoorOrientation GetDoorOrientation(Point roomQuitDirection)
        {
            return roomQuitDirection.X switch
            {
                > 0 => DoorOrientation.VerticalRight,
                < 0 => DoorOrientation.VerticalLeft,
                _ => DoorOrientation.Horizontal
            };
        }

        var roomNodes = tree.DeepCrawl()
            .Where(x => x.Room is not null)
            .ToList();

        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Hall is not null)
                )
        {
            var hall = node.Hall!;
            var i = 0;
            Point start;
            Point end;
            do
            {
                start = hall.ControlPoints[i];
                end = hall.ControlPoints[i + 1];
                i++;
            } while (hall.StartRoom.ContainsNotStrict(end));

            var direction = GetDirection(start, end);
            var startDoor = GetEdgePoint(GetDirection(start, end), hall.StartRoom, start);
            var startDoorNode = roomNodes.First(x => x.Room!.Shape.ContainsNotStrict(startDoor));
            
            startDoorNode.Room!.Doors.Add(new Door
            {
                Position = new Point(
                    startDoor.X + direction.X,
                    startDoor.Y + direction.Y
                ),
                Orientation = GetDoorOrientation(direction)
            });

            i = hall.ControlPoints.Count - 1;
            do
            {
                start = hall.ControlPoints[i - 1];
                end = hall.ControlPoints[i];

                i--;
            } while (hall.EndRoom.ContainsNotStrict(start));
            i++;

            // end и start поменяны местами т.к. end это точка, содержащаяся в hall.EndRoom
            direction = GetDirection(end, start);
            var endDoor = GetEdgePoint(direction, hall.EndRoom, end);
            var endDoorNode = roomNodes.First(x => x.Room!.Shape.ContainsNotStrict(endDoor));

            endDoorNode.Room!.Doors.Add(new Door
            {
                Position = new Point(
                    endDoor.X + direction.X,
                    endDoor.Y + direction.Y
                ),
                Orientation = GetDoorOrientation(direction)
            });
        }
    }

    private void MakeLeafs(Tree<RoomNode> tree)
    {
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Depth <= _config.ThreeDepth))
        {
            var divideType = GetDivideType(node);

            var divideProportion = _config.Random.GetDivideProportion(_config.MaxDivideShift);

            switch (divideType)
            {
                case DivideType.None:
                    continue;
                case DivideType.Vertical:
                    {
                        var leftWidth = (int)(node.Size.Width * divideProportion);
                        var rightWidth = node.Size.Width - leftWidth;

                        var height = node.Size.Height;

                        var leftLeaf = new RoomNode(node.Position, new Size(leftWidth, height));
                        var rightLeaf = new RoomNode(node.Position + new Size(leftWidth, 0), new Size(rightWidth, height));

                        node.LeftChild = leftLeaf;
                        node.RightChild = rightLeaf;
                        break;
                    }
                case DivideType.Horizontal:
                    {
                        var topHeight = (int)(node.Size.Height * divideProportion);
                        var bottomHeight = node.Size.Height - topHeight;

                        var width = node.Size.Width;

                        var leftLeaf = new RoomNode(node.Position, new Size(width, topHeight));
                        var rightLeaf = new RoomNode(node.Position + new Size(0, topHeight), new Size(width, bottomHeight));
                        node.LeftChild = leftLeaf;
                        node.RightChild = rightLeaf;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private DivideType GetDivideType(RoomNode roomNode)
    {
        var canNotBeDivided =
            roomNode.Size.Height <= _config.MinNodeSize.Height * 2 &&
            roomNode.Size.Width <= _config.MinNodeSize.Width * 2;
        if (canNotBeDivided)
        {
            return DivideType.None;
        }

        if (roomNode.Size.Width > roomNode.Size.Height
            && (double)roomNode.Size.Width / roomNode.Size.Height >= _config.MaxWidthToHeightProportion)
        {
            return DivideType.Vertical;
        }
        if (roomNode.Size.Height > roomNode.Size.Width
            && (double)roomNode.Size.Height / roomNode.Size.Width >= _config.MaxHeightToWidthProportion)
        {
            return DivideType.Horizontal;
        }

        return _config.Random.Chance(0.5) ? DivideType.Horizontal : DivideType.Vertical;
    }

    private void MakeRooms(Tree<RoomNode> tree)
    {
        foreach (var node in tree.DeepCrawl().Where(x => x.Type is TreeNodeType.Leaf))
        {
            if (_config.MinRoomSize.Width > node.Size.Width - 2 * _config.MinRoomPadding.Width ||
                _config.MinRoomSize.Height > node.Size.Height - 2 * _config.MinRoomPadding.Height)
            {
                throw new Exception("Комната с учетом отступов меньше минимального размера");
            }

            var roomSize = new Size(
                _config.Random.NextBetween(_config.MinRoomSize.Width, node.Size.Width - 2 * _config.MinRoomPadding.Width),
                _config.Random.NextBetween(_config.MinRoomSize.Height, node.Size.Height - 2 * _config.MinRoomPadding.Height)
            );
            var roomPositionShift = new Size(
                _config.Random.NextBetween(
                    _config.MinRoomPadding.Width,
                    node.Size.Width - _config.MinRoomPadding.Width - roomSize.Width
                ),
                _config.Random.NextBetween(
                    _config.MinRoomPadding.Height,
                    node.Size.Height - _config.MinRoomPadding.Height - roomSize.Height
                )
            );
            node.Room = new Room
            {
                Shape = new Rectangle(node.Position + roomPositionShift, roomSize)
            };
        }
    }

    private void MakeHalls(Tree<RoomNode> tree)
    {
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Type is TreeNodeType.Node)
                )
        {
            ConnectChildren(tree, node);
        }
    }

    private void ConnectChildren(Tree<RoomNode> tree, RoomNode node)
    {
        int GetDistance(Point a, Point b)
        {
            return int.Abs(a.X - b.X)
                   + int.Abs(a.Y - b.Y);
        }

        (RoomWithNode left, RoomWithNode right) FindClosestCenters(IEnumerable<Rectangle> left, IEnumerable<Rectangle> right)
        {
            var leftPoints = left.Select(room => new RoomWithNode(room.GetCenter(), room))
                .ToArray();
            var rightPoints = right.Select(room => new RoomWithNode(room.GetCenter(), room))
                .ToArray();

            var minDistance = GetDistance(leftPoints.First().Point, rightPoints.First().Point);
            var closestFromLeft = leftPoints.First();
            var closestFromRight = rightPoints.First();

            foreach (var leftPoint in leftPoints)
            {
                foreach (var rightPoint in rightPoints)
                {
                    if (GetDistance(leftPoint.Point, rightPoint.Point) >= minDistance) continue;

                    closestFromLeft = leftPoint;
                    closestFromRight = rightPoint;
                    minDistance = GetDistance(leftPoint.Point, rightPoint.Point);
                }
            }

            return (closestFromLeft, closestFromRight);
        }

        List<Point> CreatePath(Point a, Point b)
        {
            var c1 = new Point(a.X, b.Y);
            var c2 = new Point(b.X, a.Y);

            var strategies = new List<List<Point>>
            {
                new(),
                new(),
            };
            foreach (var strategy in strategies)
            {
                strategy.Add(a);
            }
            strategies[0].Add(c1);
            strategies[1].Add(c2);
            foreach (var strategy in strategies)
            {
                strategy.Add(b);
            }

            var strategyNumber = _config.Random.NextBetween(0, strategies.Count);
            return strategies[strategyNumber];
        }

        var leftRooms = tree.DeepCrawl(node.LeftChild!)
            .Where(x => x.Type is TreeNodeType.Leaf)
            .Select(x => x.Room!.Shape)
            .ToArray();
        var rightRooms = tree.DeepCrawl(node.RightChild!)
            .Where(x => x.Type is TreeNodeType.Leaf)
            .Select(x => x.Room!.Shape)
            .ToArray();

        var (leftResult, rightResult) = FindClosestCenters(leftRooms, rightRooms);

        node.Hall = new Hall(
            CreatePath(leftResult.Point, rightResult.Point),
            leftResult.Room,
            rightResult.Room
            );
    }

    private void NormalizeHalls(Tree<RoomNode> tree)
    {
        Point GetEdgePoint(Point direction, Rectangle room, Point innerPoint)
        {
            if (direction.Y == 0)
            {
                var edgeX = direction.X > 0
                    ? room.Right
                    : room.Left;

                return innerPoint with { X = edgeX };
            }
            var edgeY = direction.Y > 0
                ? room.Y + room.Height
                : room.Y;

            return innerPoint with { Y = edgeY };
        }

        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Hall is not null)
                )
        {
            var hall = node.Hall!;
            var i = 0;
            Point start;
            Point end;
            do
            {
                start = hall.ControlPoints[i];
                end = hall.ControlPoints[i + 1];
                i++;
            } while (hall.StartRoom.ContainsNotStrict(end));

            var newStart = GetEdgePoint(GetDirection(start, end), hall.StartRoom, start);

            hall.ControlPoints.RemoveRange(0, i - 1);
            hall.ControlPoints[0] = newStart;

            i = hall.ControlPoints.Count - 1;
            do
            {
                start = hall.ControlPoints[i - 1];
                end = hall.ControlPoints[i];

                i--;
            } while (hall.EndRoom.ContainsNotStrict(start));

            i++;
            // end и start поменяны местами т.к. end это точка, содержащаяся в EndRoom
            var newEnd = GetEdgePoint(GetDirection(end, start), hall.EndRoom, end);
            hall.ControlPoints.RemoveRange(i, hall.ControlPoints.Count - i);
            hall.ControlPoints.Add(newEnd);
        }
    }

    private void RemoveExtraControlPoints(Tree<RoomNode> tree)
    {
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Hall is not null)
                )
        {
            var hall = node.Hall!;

            var i = 0;

            while (i < hall.ControlPoints.Count - 1)
            {
                var current = hall.ControlPoints[i];
                var next = hall.ControlPoints[i + 1];
                if (current != next)
                {
                    i++;
                    continue;
                }

                hall.ControlPoints.RemoveAt(i + 1);
            }

            var j = 1;
            while (j < hall.ControlPoints.Count - 1)
            {
                var previous = hall.ControlPoints[j - 1];
                var current = hall.ControlPoints[j];
                var next = hall.ControlPoints[j + 1];

                if (!((previous.X == current.X && current.X == next.X) ||
                      (previous.Y == current.Y && current.Y == next.Y)))
                {
                    j++;
                    continue;
                }

                hall.ControlPoints.RemoveAt(j);
            }
        }
    }

    private void MarkFloor(Tree<RoomNode> tree, Map map)
    {
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Room is not null))
        {
            var room = node.Room!.Shape;
            MarkUpRandomWalkRoom(map, room);
        }

        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Hall is not null))
        {
            var points = node.Hall.ControlPoints;
            var startPoint = points.First();
            for (int i = 1; i < points.Count; i++)
            {
                var lastPoint = points[i];
                Point brush = startPoint;

                var direction = GetDirection(startPoint, lastPoint);

                map.FloorTiles.Add(new Point(brush.X, brush.Y));
                brush = new Point(brush.X + direction.X, brush.Y + direction.Y);


                while (brush.X != lastPoint.X || brush.Y != lastPoint.Y)
                {
                    map.FloorTiles.Add(new Point(brush.X, brush.Y));
                    brush = new Point(brush.X + direction.X, brush.Y + direction.Y);
                }

                map.FloorTiles.Add(new Point(brush.X, brush.Y));


                startPoint = lastPoint;
            }
        }
    }

    private void MakeDoors(Tree<RoomNode> tree, Map map)
    {
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Room is not null &&
                                 x.Room.Doors.Count > 0))
        {
            foreach (var door in node.Room!.Doors)
            {
                map.DoorTiles.Add(door.Position);

                var tiles = door.Orientation switch
                {
                    DoorOrientation.Horizontal => map.HorizontalDoors,
                    DoorOrientation.VerticalLeft => map.VerticalDoorLeft,
                    DoorOrientation.VerticalRight => map.VerticalDoorRight,
                    _ => throw new ArgumentOutOfRangeException()
                };
                tiles.Add(door.Position);
            }
        }
    }

    private static void MarkUpSimpleRoom(Map map, Rectangle room)
    {
        for (var x = room.Left; x <= room.Right; x++)
        {
            for (int y = room.Top; y <= room.Bottom; y++)
            {
                map.FloorTiles.Add(new Point(x, y));
            }
        }
    }

    private void MarkUpRandomWalkRoom(Map map, Rectangle room)
    {
        HashSet<Point> SimpleRandomWalk(Point start, int walkLength)
        {
            var path = new HashSet<Point> { start };
            var previousPosition = start;

            for (int i = 0; i < walkLength; i++)
            {
                var direction = _config.Random.NextCardinalDirection();
                if (direction.X != 0 && direction.Y != 0)
                {
                    Console.WriteLine(123);
                }
                var newPosition = new Point(
                    previousPosition.X + direction.X,
                    previousPosition.Y + direction.Y
                );
                path.Add(newPosition);
                previousPosition = newPosition;
            }

            return path;
        }

        var parameter = _config.RoomRandomWalkParameter;

        var floor = new HashSet<Point>();
        var center = room.GetCenter();

        var currentPosition = center;
        for (int i = 0; i < parameter.Iterations; i++)
        {
            var path = SimpleRandomWalk(currentPosition, parameter.WalkLength);

            floor.UnionWith(path);
            if (parameter.StartRandomlyEachIteration)
            {
                currentPosition = floor.ElementAt(_config.Random.Next(0, floor.Count));
            }
        }

        foreach (var point in floor)
        {
            if (room.ContainsNotStrict(point))
            {
                map.FloorTiles.Add(point);
            }
        }
    }

    private void MakeWalls(Map map)
    {
        var wallGenerator = new WallGenerator();
        wallGenerator.CreateWalls(map);
    }

    private Point GetDirection(Point start, Point end)
    {
        var dx = int.Sign(end.X - start.X);
        var dy = int.Sign(end.Y - start.Y);
        return new Point(
            dx,
            dy
        );
    }
}

internal record struct RoomWithNode(Point Point, Rectangle Room)
{
    public static implicit operator (Point Point, Rectangle room)(RoomWithNode value)
    {
        return (value.Point, value.Room);
    }

    public static implicit operator RoomWithNode((Point Point, Rectangle room) value)
    {
        return new RoomWithNode(value.Point, value.room);
    }
}
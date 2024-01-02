using MazeGeneration.TreeModule;
using System.Drawing;

namespace MazeGeneration;

public class MapGenerator
{
    private readonly GenerationConfig _config;
    private static readonly Point[] Directions = { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };

    public MapGenerator(GenerationConfig config)
    {
        _config = config;
    }

    public Map Generate()
    {
        var tree = new Tree<RoomNode>(new RoomNode(Point.Empty, _config.MazeSize));
        tree.Root.Depth = 1;

        var map = new Map(_config.MazeSize, tree);

        MakeLeafs(tree);
        MakeRooms(tree);
        MakeHalls(tree);
        NormalizeHalls(tree);
        RemoveExtraControlPoints(tree);

        MakeDoors(tree);
        MarkUp(tree, map);
        MarkUpDoors(tree, map);
        MakeWalls(map);

        return map;
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
            var startDoorNode = roomNodes.First(x => x.Room.Value.ContainsNotStrict(startDoor));
            startDoorNode.Doors.Add(new Point(
                startDoor.X + direction.X,
                startDoor.Y + direction.Y
            ));

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
            var endDoorNode = roomNodes.First(x => x.Room.Value.ContainsNotStrict(endDoor));

            endDoorNode.Doors.Add(new Point(
                endDoor.X + direction.X,
                endDoor.Y + direction.Y
            ));
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
            node.Room = new Rectangle(node.Position + roomPositionShift, roomSize);
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
            .Select(x => x.Room!.Value)
            .ToArray();
        var rightRooms = tree.DeepCrawl(node.RightChild!)
            .Where(x => x.Type is TreeNodeType.Leaf)
            .Select(x => x.Room!.Value)
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

    private void MarkUp(Tree<RoomNode> tree, Map map)
    {
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Room is not null))
        {
            var room = node.Room.Value;
            for (var x = room.Left; x <= room.Right; x++)
            {
                for (int y = room.Top; y <= room.Bottom; y++)
                {
                    map.FloorTiles.Add(new Point(x, y));
                    map.FloorTiles.Add(new Point(x, y));
                }
            }
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

    private void MarkUpDoors(Tree<RoomNode> tree, Map map)
    {
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Doors.Count > 0))
        {
            foreach (var door in node.Doors)
            {
                map.DoorTiles.Add(door);
            }
        }
    }

    private void MakeWalls(Map map)
    {
        foreach (var neighborPosition in map.FloorTiles
                     .SelectMany(
                         _ => Directions,
                         (floor, direction) => new Point(floor.X + direction.X, floor.Y + direction.Y))
                     .Where(neighborPosition => !map.FloorTiles.Contains(neighborPosition)))
        {
            map.WallTiles.Add(neighborPosition);
        }
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
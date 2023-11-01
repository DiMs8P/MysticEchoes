using System.Drawing;
using MazeGeneration.TreeModule;

namespace MazeGeneration;

public class MazeGenerator
{
    private readonly GenerationConfig _config;

    public MazeGenerator(GenerationConfig config)
    {
        _config = config;
    }

    public MazeGenerationResult Generate()
    {
        var maze = new Maze(_config.MazeSize);
        for (var i = 0; i < maze.Size.Height; i++)
        {
            for (var j = 0; j < maze.Size.Width; j++)
            {
                maze.Cells[i, j] = CellType.Empty;
            }
        }

        var tree = new Tree<RoomNode>(new RoomNode(Point.Empty, _config.MazeSize));
        tree.Root.Depth = 1;
        MakeLeafs(tree);
        MakeRooms(tree);
        MakeHalls(tree);
        NormalizeHalls(tree);
        RemoveExtraControlPoints(tree);

        //TODO добавить удаление лишних точек (когда 3 на одной прямой и когда какие-то две точки совпадают)
        FixHallsAndRoomsIntersection(tree);

        foreach (var node in tree.DeepCrawl())
        {
            for (var i = 0; i < node.Size.Width; i++)
            {
                maze.Cells[node.Position.Y, node.Position.X + i] = CellType.FragmentBound;
                maze.Cells[node.Position.Y + node.Size.Height - 1, node.Position.X + i] = CellType.FragmentBound;
            }
            for (var i = 0; i < node.Size.Height; i++)
            {
                maze.Cells[node.Position.Y + i, node.Position.X] = CellType.FragmentBound;
                maze.Cells[node.Position.Y + i, node.Position.X + node.Size.Width - 1] = CellType.FragmentBound;
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

                while (brush.X != lastPoint.X || brush.Y != lastPoint.Y)
                {
                    maze.Cells[brush.Y, brush.X] = CellType.Hall;
                    brush = new Point(brush.X + direction.X, brush.Y + direction.Y);
                }
                maze.Cells[brush.Y, brush.X] = CellType.Hall;
                startPoint = lastPoint;
            }
        }
        foreach (var node in tree.DeepCrawl()
                     .Where(x => x.Room is not null))
        {
            var room = node.Room.Value;
            for (var i = 0; i < room.Width; i++)
            {
                maze.Cells[room.Y, room.X + i] = CellType.Wall;
                maze.Cells[room.Y + room.Height - 1, room.X + i] = CellType.Wall;
            }
            for (var i = 0; i < room.Height; i++)
            {
                maze.Cells[room.Y + i, room.X] = CellType.Wall;
                maze.Cells[room.Y + i, room.X + room.Width - 1] = CellType.Wall;
            }
        }

        return new MazeGenerationResult(maze, tree);
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
            }
        }
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

        (RoomWithNode left, RoomWithNode right) FindClosestRoomWithPoints(IEnumerable<Rectangle> left, IEnumerable<Rectangle> right)
        {
            var leftPoints = left.Select(x => new RoomWithNode(_config.Random.NextInRectangle(x), x))
                .ToArray();
            var rightPoints = right.Select(x => new RoomWithNode(_config.Random.NextInRectangle(x), x))
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
            var r = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
            var q = new Point(r.X, a.Y);
            var p = new Point(r.X, b.Y);
            var m = new Point(a.X, r.Y);
            var n = new Point(b.X, r.Y);

            var strategies = new List<List<Point>>
            {
                new(),
                new(),
                new(),
                new(),
                new(),
                new(),
            };
            foreach (var strategy in strategies)
            {
                strategy.Add(a);
            }
            strategies[0].Add(c1);
            strategies[1].Add(c2);
            strategies[2].AddRange(new[] { q, p });
            strategies[3].AddRange(new[] { m, n });
            strategies[4].AddRange(new[] { q, r, n });
            strategies[5].AddRange(new[] { m, r, p });
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

        var (leftResult, rightResult) = FindClosestRoomWithPoints(leftRooms, rightRooms);

        node.Hall = new Hall(
            CreatePath(leftResult.Point, rightResult.Point),
            leftResult.Room,
            rightResult.Room
            );
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

    private Point GetDirection(Point start, Point end)
    {
        var dx = int.Sign(end.X - start.X);
        var dy = int.Sign(end.Y - start.Y);
        return new Point(
            dx,
            dy
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

    private void FixHallsAndRoomsIntersection(Tree<RoomNode> tree)
    {
        var rooms = tree.DeepCrawl()
                    .Where(x => x.Room is not null)
                    .Select(x => x.Room.Value)
                    .ToArray();
        foreach (var hall in tree.DeepCrawl()
                     .Where(x => x.Hall is not null)
                     .Select(x => x.Hall))
        {
            var controlPoints = hall.ControlPoints;
            for (var i = 1; i < controlPoints.Count - 1; i++)
            {
                var current = controlPoints[i];

                Rectangle intersectedRoom;
                try
                {
                    intersectedRoom = rooms.First(x => x.Contains(current));
                }
                catch (InvalidOperationException)
                {
                    continue;
                }

                var previous = controlPoints[i - 1];
                var next = controlPoints[i + 1];

                var previousToCurrentDirection = GetDirection(previous, current);
                var newPrevious = previousToCurrentDirection switch
                {
                    { X: 1, Y: 0 } => previous with { X = intersectedRoom.Left - 1 },
                    { X: -1, Y: 0 } => previous with { X = intersectedRoom.Right + 1 },
                    { X: 0, Y: 1 } => previous with { Y = intersectedRoom.Top - 1 },
                    { X: 0, Y: -1 } => previous with { Y = intersectedRoom.Bottom + 1 },
                    _ => throw new Exception()
                };
                controlPoints.Insert(i, newPrevious);

                var currentToNextDirection = GetDirection(current, next);
                var newNext = currentToNextDirection switch
                {
                    { X: 1, Y: 0 } => next with { X = intersectedRoom.Right + 1 },
                    { X: -1, Y: 0 } => next with { X = intersectedRoom.Left - 1 },
                    { X: 0, Y: 1 } => next with { Y = intersectedRoom.Bottom + 1 },
                    { X: 0, Y: -1 } => next with { Y = intersectedRoom.Top - 1 },
                    _ => throw new Exception()
                };
                controlPoints.Insert(i + 2, newNext);

                var oldPointShift = (Size)((newPrevious - (Size)current) + (Size)(newNext - (Size)current));

                controlPoints[i + 1] = current + oldPointShift;
            }
        }

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

public record MazeGenerationResult(Maze Maze, Tree<RoomNode> Tree);
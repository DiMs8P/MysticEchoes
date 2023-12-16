using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.Collisions;

public class QuadTree
{
    private bool IsDivided => _subTrees.Count > 0;
    private readonly Rectangle _boundary;
    private readonly int _capacity;
    private readonly List<Box> _boxes = new();
    private readonly Dictionary<AreaType, QuadTree> _subTrees = new ();
    private int Depth;

    public QuadTree(Rectangle boundary, int capacity)
    {
        _boundary = boundary;
        _capacity = capacity;
        Depth = 0;
    }

    private QuadTree(Rectangle boundary, int capacity, int depth)
    {
        _boundary = boundary;
        _capacity = capacity;
        Depth = depth;
    }

    public void Add(Box area)
    {
        if (!_boundary.Intersects(area.Shape))
        {
            return;
        }

        if (!IsDivided)
        {
            if (_boxes.Count < _capacity)
            {
                _boxes.Add(area);
                return;
            }

            Subdivide();
        }

        foreach (var kv in _subTrees)
        {
            kv.Value.Add(area);
        }
    }

    public HashSet<int> Query(Rectangle rectangle)
    {
        var result = new HashSet<int>();

        foreach (var subTree in _subTrees.Values)
        {
            subTree.Query(rectangle, result);
        }

        return result;
    }

    private void Query(Rectangle rectangle, HashSet<int> intersectWith)
    {
        if (!IsDivided)
        {
            foreach (var box in _boxes.Where(box => rectangle.Intersects(box.Shape)))
            {
                intersectWith.Add(box.Id);
            }
            return;
        }

        foreach (var subTree in _subTrees.Values)
        {
            subTree.Query(rectangle, intersectWith);
        }
    }

    public int MaxDepthQuery()
    {
        if (!IsDivided)
            return Depth;

        var result = _subTrees.Values.Aggregate(-1, (current, subTree) => 
            int.Max(current, subTree.MaxDepthQuery())
        );

        return result;
    }

    private void Subdivide()
    {
        _subTrees.Add(AreaType.LeftBottom, new QuadTree(
            new Rectangle(_boundary.LeftBottom, _boundary.Size / 2),
            _capacity,
            Depth + 1
        ));
        _subTrees.Add(AreaType.LeftTop, new QuadTree(
            new Rectangle(
                _boundary.LeftBottom + (_boundary.Size / 2) with { X = 0 }, 
                _boundary.Size / 2),
            _capacity,
            Depth + 1
        ));
        _subTrees.Add(AreaType.RightBottom, new QuadTree(
            new Rectangle(
                _boundary.LeftBottom + (_boundary.Size / 2) with { Y = 0 },
                _boundary.Size / 2),
            _capacity,
            Depth + 1
        ));
        _subTrees.Add(AreaType.RightTop, new QuadTree(
            new Rectangle(
                _boundary.LeftBottom + _boundary.Size / 2,
                _boundary.Size / 2),
            _capacity,
            Depth + 1
        ));

        foreach (var area in _boxes)
        {
            foreach (var subTree in _subTrees.Values)
            {
                subTree.Add(area);
            }
        }
        _boxes.Clear();
    }

    public override string ToString()
    {
        var height = MaxDepthQuery() - Depth;
        return height != 0 
            ? $"Height={height}|{_boundary}" 
            : $"Boxes={_boxes.Count}|{_boundary}";
    }
}
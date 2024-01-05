using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.Collisions.Tree;

public class QuadTree
{
    private bool IsDivided => _subTrees.Count > 0;
    public Rectangle Bound { get; }
    public List<QuadTree> SubTrees => _subTrees.Select(x => x.Value).ToList();

    private readonly int _capacity;
    private readonly int _maxDepth;
    private List<Box> _boxes = new();
    private Dictionary<AreaType, QuadTree> _subTrees = new ();
    private readonly int _depth;

    public QuadTree(Rectangle bound, int capacity, int maxDepth = 6)
    {
        Bound = bound;
        _capacity = capacity;
        _maxDepth = maxDepth;
        _depth = 0;
    }

    private QuadTree(Rectangle bound, int capacity, int depth, int maxDepth)
    {
        Bound = bound;
        _capacity = capacity;
        _depth = depth;
        _maxDepth = maxDepth;
    }

    public void Add(Box area)
    {
        if (!Bound.Intersects(area.Shape))
        {
            return;
        }

        if (!IsDivided)
        {
            if (_boxes.Count < _capacity || _depth == _maxDepth)
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
    
    public void Remove(Box area)
    {
        if (_boxes.Contains(area))
        {
            _boxes.Remove(area);
        }

        foreach (var kv in _subTrees)
        {
            kv.Value.Remove(area);
        }
    }

    public void Clear()
    {
        _subTrees = new();
        _boxes = new();
    }

    public HashSet<int> Query(Rectangle rectangle)
    {
        var result = new HashSet<int>();

        Query(rectangle,  result);

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
            return _depth;

        var result = _subTrees.Values.Aggregate(-1, (current, subTree) => 
            int.Max(current, subTree.MaxDepthQuery())
        );

        return result;
    }

    private void Subdivide()
    {
        _subTrees.Add(AreaType.LeftBottom, new QuadTree(
            new Rectangle(Bound.LeftBottom, Bound.Size / 2),
            _capacity,
            _depth + 1,
            _maxDepth
        ));
        _subTrees.Add(AreaType.LeftTop, new QuadTree(
            new Rectangle(
                Bound.LeftBottom + (Bound.Size / 2) with { X = 0 }, 
                Bound.Size / 2),
            _capacity,
            _depth + 1,
            _maxDepth
        ));
        _subTrees.Add(AreaType.RightBottom, new QuadTree(
            new Rectangle(
                Bound.LeftBottom + (Bound.Size / 2) with { Y = 0 },
                Bound.Size / 2),
            _capacity,
            _depth + 1,
            _maxDepth
        ));
        _subTrees.Add(AreaType.RightTop, new QuadTree(
            new Rectangle(
                Bound.LeftBottom + Bound.Size / 2,
                Bound.Size / 2),
            _capacity,
            _depth + 1,
            _maxDepth
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
        var height = MaxDepthQuery() - _depth;
        return height != 0 
            ? $"Height={height}|{Bound}" 
            : $"Boxes={_boxes.Count}|{Bound}";
    }
}
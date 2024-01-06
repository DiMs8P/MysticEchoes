namespace MysticEchoes.Core.AI.BehaviorTree;

public abstract class Tree
{
    private Node _root = null;

    public void Start()
    {
        _root = SetupTree();
    }

    public void Update()
    {
        if (_root is not null)
        {
            _root.Evaluate();
        }
    }

    protected abstract Node SetupTree();
}
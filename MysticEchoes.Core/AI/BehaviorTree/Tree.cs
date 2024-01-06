namespace MysticEchoes.Core.AI.BehaviorTree;

public abstract class Tree
{
    private Node _root = null;

    protected void Start()
    {
        _root = SetupTree();
    }

    private void Update()
    {
        if (_root is not null)
        {
            _root.Evaluate();
        }
    }

    protected abstract Node SetupTree();
}
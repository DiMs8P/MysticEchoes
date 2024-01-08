namespace MysticEchoes.Core.AI.BehaviorTree;

public abstract class Tree
{
    protected Blackboard Blackboard;
    private Node _root = null;
    protected Tree()
    {
        Blackboard = new Blackboard();
    }
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
namespace MysticEchoes.Core.AI.BehaviorTree;

public class Node
{
    protected NodeState State;

    protected Node Parent;
    protected List<Node> Children = new List<Node>();

    public Node()
    {
        Parent = null;
    }

    public Node(List<Node> children)
    {
        foreach (var child in children)
        {
            Attach(child);
        }
    }

    private void Attach(Node node)
    {
        node.Parent = this;
        Children.Add(node);
    }

    public virtual NodeState Evaluate() => NodeState.Failure;
}
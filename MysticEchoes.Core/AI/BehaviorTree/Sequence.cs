namespace MysticEchoes.Core.AI.BehaviorTree;

public class Sequence : Node
{
    public Sequence() : base() {}
    public Sequence(List<Node> children) : base(children){}
    
    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;
        NodeState state = NodeState.None;

        foreach (Node node in Children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    anyChildIsRunning = true;
                    continue;
                default:
                    state = NodeState.Success;
                    return state;
            }
        }

        state = anyChildIsRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}
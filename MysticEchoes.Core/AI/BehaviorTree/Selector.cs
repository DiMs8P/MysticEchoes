namespace MysticEchoes.Core.AI.BehaviorTree;

public class Selector : Node
{
    public Selector() : base() {}
    public Selector(List<Node> children) : base(children){}
    
    public override NodeState Evaluate()
    {
        NodeState state = NodeState.None;

        foreach (Node node in Children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    continue;
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                default:
                    continue;
            }
        }

        state = NodeState.Failure;
        return state;
    }
}
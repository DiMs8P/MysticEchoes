namespace MysticEchoes.Core.AI.BehaviorTree;

public class Sequence : Node
{
    public Sequence() : base() {}
    public Sequence(List<Node> children) : base(children){}
    
    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;

        foreach (Node node in Children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    State = NodeState.Failure;
                    return State;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    anyChildIsRunning = true;
                    continue;
                default:
                    State = NodeState.Success;
                    return State;
            }
        }

        State = anyChildIsRunning ? NodeState.Running : NodeState.Success;
        return State;
    }
}
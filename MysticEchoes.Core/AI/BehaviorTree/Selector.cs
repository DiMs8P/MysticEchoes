﻿namespace MysticEchoes.Core.AI.BehaviorTree;

public class Selector : Node
{
    public Selector() : base() {}
    public Selector(List<Node> children) : base(children){}
    
    public override NodeState Evaluate()
    {
        foreach (Node node in Children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    continue;
                case NodeState.Success:
                    State = NodeState.Success;
                    return State;
                case NodeState.Running:
                    State = NodeState.Running;
                    return State;
                default:
                    continue;
            }
        }

        State = NodeState.Failure;
        return State;
    }
}
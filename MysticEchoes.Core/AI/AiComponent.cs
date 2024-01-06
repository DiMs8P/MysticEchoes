using MysticEchoes.Core.AI.BehaviorTree;

namespace MysticEchoes.Core.AI;

public struct AiComponent
{
    public Tree BehaviorTree { get; set; }

    public AiComponent()
    {
        BehaviorTree = null;
    }
}
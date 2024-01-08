using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;

namespace MysticEchoes.Core.AI.Ecs;

public class EcsNode : Node
{
    protected EcsWorld World;
    protected int SelfEntityId;
    protected Blackboard Blackboard;
    
    public EcsNode(Blackboard blackboard)
    {
        Blackboard = blackboard;
        
        World = (EcsWorld)blackboard.GetValueAsObject("World");
        SelfEntityId = blackboard.GetValueAsInt("SelfEntityId");
    }
}
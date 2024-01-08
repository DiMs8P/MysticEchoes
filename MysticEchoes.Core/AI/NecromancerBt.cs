using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Decorators;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.AI.Services;
using MysticEchoes.Core.AI.Tasks;
using MysticEchoes.Core.Player;

namespace MysticEchoes.Core.AI;

public class NecromancerBt : EcsBt
{
    public NecromancerBt(EcsWorld world, int ownerEntityId) : base(world, ownerEntityId)
    {
    }
    protected override Node SetupTree()
    {
        Node root = new Sequence(new List<Node>
        {
            new SetPlayerTarget(Blackboard, "Player"),
            new TaskMoveTo(Blackboard, "Player", 0.1f),
            new SetHasAim(Blackboard, "Player",0.11f),
            new Attack(Blackboard),
        });
        
        return root;
    }
}
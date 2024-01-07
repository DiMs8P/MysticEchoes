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
    private readonly int _playerId;
    public NecromancerBt(EcsWorld world, int ownerEntityId) : base(world, ownerEntityId)
    {
        EcsFilter playerFilter = world.Filter<PlayerMarker>().End();

        foreach (var playerId in playerFilter)
        {
            _playerId = playerId;
        }
        Blackboard.SetValueAsInt("Player", _playerId);
    }
    protected override Node SetupTree()
    {
        Node root = new Sequence(new List<Node>
        {
            new TaskMoveTo(Blackboard, "Player", 0.1f),
            new SetHasAim(Blackboard, "Player",0.11f),
            new Attack(Blackboard),
        });
        
        return root;
    }
}
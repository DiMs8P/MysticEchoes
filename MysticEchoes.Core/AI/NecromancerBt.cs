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
    }
    protected override Node SetupTree()
    {
        
        Node root = new Sequence(new List<Node>
        {
            new TaskMoveTo(_world, _ownerEntityId, _playerId, 0.1f),
            new SetHasAim(_world, _ownerEntityId, _playerId, 0.11f),
            new Attack(_world, _ownerEntityId),
        });
        
        return root;
    }
}
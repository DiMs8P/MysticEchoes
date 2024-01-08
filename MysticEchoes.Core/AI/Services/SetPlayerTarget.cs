using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Ecs;
using MysticEchoes.Core.Player;

namespace MysticEchoes.Core.AI.Services;

public class SetPlayerTarget : EcsNode
{
    private EcsFilter _playerFilter;

    private string _playerKey;
    
    public SetPlayerTarget(Blackboard blackboard, string playerKey) : base(blackboard)
    {
        _playerFilter = World.Filter<PlayerMarker>().End();

        _playerKey = playerKey;
    }

    public override NodeState Evaluate()
    {
        int playerId = -1;
        foreach (var playerEntityId in _playerFilter)
        {
            playerId = playerEntityId;
        }

        if (playerId != -1)
        {
            Blackboard.SetValueAsInt(_playerKey, playerId);
            State = NodeState.Success;
        }
        else
        {
            State = NodeState.Failure;
        }

        return State;
    }
}
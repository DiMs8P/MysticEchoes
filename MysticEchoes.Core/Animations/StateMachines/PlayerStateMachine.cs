using Leopotam.EcsLite;

namespace MysticEchoes.Core.Animations.StateMachines;

public class PlayerStateMachine : CharacterStateMachine
{
    public PlayerStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
    }

    // TODO описать стояние на месте
    public override CharacterState Update()
    {
        return base.Update();
    }
}
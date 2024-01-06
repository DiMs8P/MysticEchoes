using Leopotam.EcsLite;

namespace MysticEchoes.Core.Animations.StateMachines;

public class CharacterStateMachine : BaseStateMachine
{
    public CharacterStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
    }

    // TODO описать всё кроме стояния на месте
    public override CharacterState Update()
    {
        return CharacterState.Idle;
    }
}
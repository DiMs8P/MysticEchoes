using Leopotam.EcsLite;

namespace MysticEchoes.Core.Animations.StateMachines;

public class NecromancerStateMachine : BaseStateMachine
{
    public NecromancerStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
    }

    public override void Update()
    {
        return;
    }

    public override CharacterState GetCurrentState()
    {
        throw new NotImplementedException();
    }
}
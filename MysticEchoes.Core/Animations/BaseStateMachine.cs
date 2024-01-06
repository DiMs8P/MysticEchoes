using Leopotam.EcsLite;

namespace MysticEchoes.Core.Animations;

public abstract class BaseStateMachine
{
    protected EcsWorld World;
    protected int OwnerEntityId;

    protected BaseStateMachine(int ownerEntityId, EcsWorld world)
    {
        World = world;
        OwnerEntityId = ownerEntityId;
    }

    public abstract CharacterState Update();
}
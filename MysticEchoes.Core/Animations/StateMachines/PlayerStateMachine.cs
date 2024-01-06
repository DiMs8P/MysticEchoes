﻿using Leopotam.EcsLite;

namespace MysticEchoes.Core.Animations.StateMachines;

public class PlayerStateMachine : BaseStateMachine
{
    public PlayerStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
    }

    public override CharacterState Update()
    {
        return CharacterState.Idle;
    }
}
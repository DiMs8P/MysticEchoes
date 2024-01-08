using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.Animations.StateMachines;

public class BringerOfDeathStateMachine : IdleRunShootingHitDeathStateMachine
{
    private EcsPool<AnimationComponent> _animations;

    public BringerOfDeathStateMachine(int ownerEntityId, EcsWorld world) : base(ownerEntityId, world)
    {
        _animations = world.GetPool<AnimationComponent>();
    }

    public override void Update()
    {
        base.Update();
        
        ref AnimationComponent animationComponent = ref _animations.Get(OwnerEntityId);
        animationComponent.ReflectByY = !animationComponent.ReflectByY;
    }
}
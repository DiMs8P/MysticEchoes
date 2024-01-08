using Leopotam.EcsLite;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.Animations.AnimNotifies;

public class OnRemoveNotify : AnimNotify
{
    public override void Notify(int animationOwnerEntityId, EcsWorld world)
    {
        EcsPool<LifeTimeComponent> lifetimes = world.GetPool<LifeTimeComponent>();

        ref LifeTimeComponent lifeTimeComponent = ref lifetimes.Has(animationOwnerEntityId)
            ? ref lifetimes.Get(animationOwnerEntityId)
            : ref lifetimes.Add(animationOwnerEntityId);

        lifeTimeComponent.LifeTime = 0.0f;
        lifeTimeComponent.IsActive = true;
    }
}
using Leopotam.EcsLite;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.Animations.AnimNotifies;

public class OnRemoveNotify : AnimNotify
{
    public override void Notify(int animationOwnerEntityId, EcsWorld world)
    {
        EcsPool<LifeTimeComponent> lifetimes = world.GetPool<LifeTimeComponent>();

        if (lifetimes.Has(animationOwnerEntityId))
        {
            ref LifeTimeComponent lifeTimeComponent = ref lifetimes.Get(animationOwnerEntityId);
            lifeTimeComponent.LifeTime = 0.0f;
            lifeTimeComponent.IsActive = true;
        }
        else
        {
            lifetimes.Add(animationOwnerEntityId) = new LifeTimeComponent()
            {
                LifeTime = 0.0f,
                IsActive = true
            };
        }
    }
}
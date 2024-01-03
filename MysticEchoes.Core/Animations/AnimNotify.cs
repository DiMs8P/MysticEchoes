using Leopotam.EcsLite;

namespace MysticEchoes.Core.Animations;

public abstract class AnimNotify
{
    public abstract void Notify(int animationOwnerEntityId, EcsWorld world);
}
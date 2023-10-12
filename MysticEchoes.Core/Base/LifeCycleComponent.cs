namespace MysticEchoes.Core.Base;

public abstract class LifeCycleComponent : IComponent
{
    protected Entity Entity { get; private set; } = null!;

    public virtual void Initialize(Entity entity)
    {
        Entity = entity;
    }

    public virtual void Update() {}
}
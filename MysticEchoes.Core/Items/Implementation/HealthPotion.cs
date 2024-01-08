using Leopotam.EcsLite;
using MysticEchoes.Core.Health;

namespace MysticEchoes.Core.Items.Implementation;

public class HealthPotion : BaseItem
{
    private readonly int _value;

    public HealthPotion(int value)
    {
        _value = value;
    }

    public override void OnItemTaken(int instigator, EcsWorld world)
    {
        EcsPool<HealthComponent> healthPool = world.GetPool<HealthComponent>();
        ref HealthComponent healthComponent = ref healthPool.Get(instigator);

        healthComponent.Health += float.Min(healthComponent.Health + _value, healthComponent.MaxHealth);
    }
}
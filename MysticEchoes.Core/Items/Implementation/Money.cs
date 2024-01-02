using Leopotam.EcsLite;
using MysticEchoes.Core.Inventory;

namespace MysticEchoes.Core.Items.Implementation;

public class Money : BaseItem
{
    private readonly int _value;

    public Money(int value)
    {
        _value = value;
    }

    public override void OnItemTaken(int instigator, EcsWorld world)
    {
        EcsPool<InventoryComponent> inventoryPool = world.GetPool<InventoryComponent>();
        ref InventoryComponent inventoryComponent = ref inventoryPool.Get(instigator);

        inventoryComponent.Money += _value;
    }
}
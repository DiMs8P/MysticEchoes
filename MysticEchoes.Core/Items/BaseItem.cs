using Leopotam.EcsLite;
using MysticEchoes.Core.Inventory;

namespace MysticEchoes.Core.Items;

public class BaseItem : IPickableItem
{
    public void OnItemTaken(int itemEntityId, int instigator, EcsWorld world)
    {
        EcsPool<InventoryComponent> inventoryPool = world.GetPool<InventoryComponent>();
        ref InventoryComponent inventoryComponent = ref inventoryPool.Get(instigator);
        
        inventoryComponent.TakenItems.Add(this);
    }

    protected void Destroy(int itemEntityId, EcsWorld world)
    {
        world.DelEntity(itemEntityId);
    }
}
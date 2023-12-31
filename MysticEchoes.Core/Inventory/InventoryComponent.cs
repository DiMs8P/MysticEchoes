using MysticEchoes.Core.Items;

namespace MysticEchoes.Core.Inventory;

public struct InventoryComponent
{
    public uint Money { get; set; }
    //public Dictionary<Item, uint> ConsumableItems { get; set; }
    public List<Item> TakenItems { get; set; }

    public InventoryComponent()
    {
        Money = 0;
        //ConsumableItems = new Dictionary<Item, uint>();
        TakenItems = new List<Item>();
    }
}
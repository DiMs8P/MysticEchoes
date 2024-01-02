using MysticEchoes.Core.Items;

namespace MysticEchoes.Core.Inventory;

public struct InventoryComponent
{
    public int Money { get; set; }
    //public Dictionary<BaseItem, uint> ConsumableItems { get; set; }
    public List<BaseItem> TakenItems { get; set; }
    public List<MagicAffectedItem> MagicAffectedItems { get; set; }

    public InventoryComponent()
    {
        Money = 0;
        //ConsumableItems = new Dictionary<BaseItem, uint>();
        TakenItems = new List<BaseItem>();
        MagicAffectedItems = new List<MagicAffectedItem>();
    }
}
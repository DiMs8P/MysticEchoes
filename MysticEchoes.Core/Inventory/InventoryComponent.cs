namespace MysticEchoes.Core.Inventory;

public struct InventoryComponent
{
    public int Money { get; set; }
    //public Dictionary<uint, uint> ConsumableItems { get; set; }
    public List<uint> TakenItems { get; set; }

    public InventoryComponent()
    {
        Money = 0;
        //ConsumableItems = new Dictionary<uint, uint>();
        TakenItems = new List<uint>();
    }
}
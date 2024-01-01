using MysticEchoes.Core.Items;

namespace MysticEchoes.Core.Inventory;

public struct StartingItems
{
    public List<Item> Items { get; set; }

    public StartingItems()
    {
        Items = new List<Item>();
    }
}
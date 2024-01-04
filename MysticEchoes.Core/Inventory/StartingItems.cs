using MysticEchoes.Core.Items;

namespace MysticEchoes.Core.Inventory;

public struct StartingItems
{
    public List<int> Items { get; set; }

    public StartingItems()
    {
        Items = new List<int>();
    }
}
using MysticEchoes.Core.Items;

namespace MysticEchoes.Core.Configuration;

public class ItemsSettings
{
    public Dictionary<uint, ItemInfo> Items { get; set; }
    public Dictionary<ItemQuality, uint> QualityPoints { get; set; }
}
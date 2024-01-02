namespace MysticEchoes.Core.Items;

public struct ItemComponent
{
    public BaseItem Item { get; set; }

    public ItemComponent()
    {
        Item = new BaseItem();
    }
}
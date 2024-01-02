using MysticEchoes.Core.Items.Implementation;

namespace MysticEchoes.Core.Items;

public class ItemsFactory
{
    private static readonly Dictionary<Item, Func<object[], BaseItem>> FactoryMethods
        = new Dictionary<Item, Func<object[], BaseItem>>();

    static ItemsFactory()
    {
        FactoryMethods.Add(Item.Money, parameters => new Money((int)parameters[0]));
        FactoryMethods.Add(Item.Laser, _ => new LaserItem());
        FactoryMethods.Add(Item.CricketsHead, _ => new CricketsHead());
    }

    public static BaseItem CreateItem(Item itemId, params object[] parameters)
    {
        if (FactoryMethods.TryGetValue(itemId, out Func<object[], BaseItem> factoryMethod))
        {
            return factoryMethod(parameters);
        }

        throw new ArgumentException($"Invalid item Id: {itemId.ToString()}");
    }
}
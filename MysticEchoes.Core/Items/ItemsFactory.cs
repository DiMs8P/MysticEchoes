using MysticEchoes.Core.Items.Implementation;

namespace MysticEchoes.Core.Items;

public class ItemsFactory
{
    private static readonly Dictionary<uint, Func<object[], BaseItem>> FactoryMethods
        = new Dictionary<uint, Func<object[], BaseItem>>();

    static ItemsFactory()
    {
        FactoryMethods.Add(0, parameters => new Money((uint)parameters[0]) { EntityId = 0 });
        FactoryMethods.Add(1, _ => new LaserItem() { EntityId = 1 });
    }

    public static BaseItem CreateItem(uint id, params object[] parameters)
    {
        if (FactoryMethods.TryGetValue(id, out Func<object[], BaseItem> factoryMethod))
        {
            return factoryMethod(parameters);
        }

        throw new ArgumentException($"Invalid item Id: {id}");
    }
}
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Items.Implementation;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.Items;

public class ItemsFactory
{
    private readonly EcsPool<SpriteComponent> _sprites;
    private readonly EcsPool<ItemComponent> _items;
    
    private readonly EntityFactory _factory;
    private readonly PrefabManager _prefabManager;
    private readonly ItemsSettings _itemsSettings;
    
    private static readonly Dictionary<Item, Func<object[], BaseItem>> FactoryMethods
        = new Dictionary<Item, Func<object[], BaseItem>>();

    public ItemsFactory(EcsWorld world, EntityFactory factory, PrefabManager prefabManager, ItemsSettings itemsSettings)
    {
        _sprites = world.GetPool<SpriteComponent>();
        _items = world.GetPool<ItemComponent>();

        _factory = factory;
        _prefabManager = prefabManager;
        _itemsSettings = itemsSettings;

        InitializeFactoryMethods();
    }

    private void InitializeFactoryMethods()
    {
        FactoryMethods.Add(Item.Money, parameters => new Money((int)parameters[0]));
        FactoryMethods.Add(Item.Laser, _ => new LaserItem());
        FactoryMethods.Add(Item.CricketsHead, _ => new CricketsHead());
    }

    public BaseItem CreateItem(Item itemId, params object[] parameters)
    {
        if (FactoryMethods.TryGetValue(itemId, out Func<object[], BaseItem> factoryMethod))
        {
            return factoryMethod(parameters);
        }

        throw new ArgumentException($"Invalid item Id: {itemId.ToString()}");
    }
    
    public int CreateItemEntity(Item itemId, params object[] parameters)
    {
        int itemEntity = _prefabManager.CreateEntityFromPrefab(_factory, PrefabType.BaseItem);
        
        if (_itemsSettings.Items.TryGetValue(Item.Money, out ItemInfo itemInfo))
        {
            if (itemInfo.AnimationId is not null)
            {
                AnimationComponent animationComponent = new AnimationComponent()
                {
                    AnimationId = itemInfo.AnimationId
                };

                _factory.AddTo(itemEntity, animationComponent);
            }

            ref SpriteComponent spriteComponent = ref _sprites.Get(itemEntity);
            spriteComponent.Sprite = itemInfo.SpriteId;
        }
        
        ref ItemComponent itemComponent = ref _items.Get(itemEntity);
        itemComponent.Item =  CreateItem(itemId, parameters);

        return itemEntity;
    }
}
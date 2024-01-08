using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Items.Implementation;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.Items;

public class ItemsFactory
{
    private readonly EcsPool<SpriteComponent> _sprites;
    private readonly EcsPool<ItemComponent> _items;
    
    private readonly EcsPool<TransformComponent> _transforms;
    private readonly EcsPool<DynamicCollider> _dynamicColliders;
    
    private readonly EntityBuilder _builder;
    private readonly PrefabManager _prefabManager;
    private readonly ItemsSettings _itemsSettings;
    
    private static readonly Dictionary<int, Func<object[], BaseItem>> FactoryMethods
        = new Dictionary<int, Func<object[], BaseItem>>();

    public ItemsFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager, ItemsSettings itemsSettings)
    {
        _sprites = world.GetPool<SpriteComponent>();
        _items = world.GetPool<ItemComponent>();

        _transforms = world.GetPool<TransformComponent>();
        _dynamicColliders = world.GetPool<DynamicCollider>();

        _builder = builder;
        _prefabManager = prefabManager;
        _itemsSettings = itemsSettings;

        InitializeFactoryMethods();
    }

    private void InitializeFactoryMethods()
    {
        FactoryMethods.Add(0, parameters => new Money((int)parameters[0]));
        FactoryMethods.Add(1, _ => new LaserItem());
        FactoryMethods.Add(2, _ => new CricketsHead());
        FactoryMethods.Add(3, parameters => new HealthPotion((int)parameters[0]));
    }

    public BaseItem CreateItem(int itemId, params object[] parameters)
    {
        if (FactoryMethods.TryGetValue(itemId, out Func<object[], BaseItem> factoryMethod))
        {
            return factoryMethod(parameters);
        }

        throw new ArgumentException($"Invalid item Id: {itemId.ToString()}");
    }
    
    public int CreateItemEntity(ItemInitializationInfo initializationInfo, params object[] parameters)
    {
        int itemEntity = _prefabManager.CreateEntityFromPrefab(_builder, PrefabType.BaseItem);
        
        if (_itemsSettings.Items.TryGetValue(initializationInfo.ItemId, out ItemInfo itemInfo))
        {
            if (itemInfo.AnimationId is not null)
            {
                AnimationComponent animationComponent = new AnimationComponent()
                {
                    AnimationId = itemInfo.AnimationId
                };

                _builder.AddTo(itemEntity, animationComponent);
            }

            ref SpriteComponent spriteComponent = ref _sprites.Get(itemEntity);
            spriteComponent.Sprite = itemInfo.SpriteId;
        }
        
        ref ItemComponent itemComponent = ref _items.Get(itemEntity);
        itemComponent.Item =  CreateItem(initializationInfo.ItemId, parameters);
        
        ref TransformComponent itemTransform = ref _transforms.Get(itemEntity);
        itemTransform.Location = initializationInfo.Location;

        ref DynamicCollider colliderHealth = ref _dynamicColliders.Get(itemEntity);
        colliderHealth.Box = new Box(itemEntity, new Rectangle(
            Vector2.Zero,
            colliderHealth.DefaultSize * itemTransform.Scale
        ));
        colliderHealth.Behavior = CollisionBehavior.Item;

        return itemEntity;
    }
}
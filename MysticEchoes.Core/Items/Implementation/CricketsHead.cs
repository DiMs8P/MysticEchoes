using Leopotam.EcsLite;
using MysticEchoes.Core.Inventory;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Shooting;

namespace MysticEchoes.Core.Items.Implementation;

public class CricketsHead : MagicAffectedItem
{
    public override void Apply(int magicId, EcsWorld world)
    {
        EcsPool<TransformComponent> transforms = world.GetPool<TransformComponent>();
        EcsPool<DamageComponent> damages = world.GetPool<DamageComponent>();

        ref TransformComponent projectileComponent = ref transforms.Get(magicId);
        projectileComponent.Scale *= 1.5f;
        
        ref DamageComponent damageComponent = ref damages.Get(magicId);
        damageComponent.Damage *= 1.5f;
        
        base.Apply(magicId, world);
    }

    public override void OnItemTaken(int instigator, EcsWorld world)
    {
        EcsPool<InventoryComponent> inventoryPool = world.GetPool<InventoryComponent>();
        ref InventoryComponent inventoryComponent = ref inventoryPool.Get(instigator);
        
        inventoryComponent.MagicAffectedItems.Add(this);
        
        base.OnItemTaken(instigator, world);
    }
}
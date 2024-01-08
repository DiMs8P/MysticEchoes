using Leopotam.EcsLite;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Damage;

namespace MysticEchoes.Core.Animations.AnimNotifies;

public class OnEnableDamageZoneNotify : AnimNotify
{
    public override void Notify(int animationOwnerEntityId, EcsWorld world)
    {
        SetupDynamicCollider(animationOwnerEntityId, world);
        SetupDamageZone(animationOwnerEntityId, world);
    }

    private void SetupDynamicCollider(int entityId, EcsWorld world)
    {
        EcsPool<DynamicCollider> dynamicColliders = world.GetPool<DynamicCollider>();
        
        if (!dynamicColliders.Has(entityId))
        {
            throw new Exception($"Entity {entityId} must have DynamicCollider component.");
        }
        
        ref DynamicCollider dynamicCollider = ref dynamicColliders.Get(entityId);
        dynamicCollider.Behavior = CollisionBehavior.DamageZone;
    }
    
    private void SetupDamageZone(int animationOwnerEntityId, EcsWorld world)
    {
        EcsPool<DamageZoneComponent> damageZones = world.GetPool<DamageZoneComponent>();

        if (damageZones.Has(animationOwnerEntityId))
        {
            ref DamageZoneComponent damageZoneComponent = ref damageZones.Get(animationOwnerEntityId);
            damageZoneComponent.EntityToDamage.Clear();
            damageZoneComponent.DamagedEntities.Clear();
        }
        else
        {
            DamageZoneComponent damageZone = new DamageZoneComponent();
            damageZones.Add(animationOwnerEntityId) = damageZone;
        }
    }
}
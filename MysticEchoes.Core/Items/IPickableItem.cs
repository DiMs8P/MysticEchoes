using Leopotam.EcsLite;

namespace MysticEchoes.Core.Items;

public interface IPickableItem
{
    void OnItemTaken(int itemEntityId, int instigator, EcsWorld world);
}
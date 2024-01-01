using Leopotam.EcsLite;

namespace MysticEchoes.Core.Items;

public interface IPickableItem
{
    void OnItemTaken(int instigator, EcsWorld world);
}
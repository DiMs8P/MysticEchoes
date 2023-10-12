using MysticEchoes.Core.Base;

namespace MysticEchoes.Core;

public class BehaviorSystem : ISystem
{
    public void Update(Entity entity)
    {
        var behavior = entity.GetComponent<BehaviorComponent>();

        // Обработка поведения врагов в зависимости от их типов
        // ...
    }
}
using MysticEchoes.Core.Base;

namespace MysticEchoes.Core;

public class PlayerMovementSystem : ISystem
{
    public void Update(Entity entity)
    {
        var transform = entity.GetComponent<TransformComponent>();

        // Обработка пользовательского ввода (WASD)
        // ...

        // Перемещение игрока в соответствии с пользовательским вводом
        // ...
    }
}
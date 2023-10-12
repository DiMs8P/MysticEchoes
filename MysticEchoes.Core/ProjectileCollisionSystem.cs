using MysticEchoes.Core.Base;

namespace MysticEchoes.Core;

public class ProjectileCollisionSystem : ISystem
{
    public void Update(Entity entity)
    {
        var hitBoxComponent = entity.GetComponent<HitBoxComponent>();
        var projectileComponent = entity.GetComponent<ProjectileComponent>();

        // Обработка попадания снаряда по игроку/врагу в зависимости от их хитбоксов и типов снарядов
        // ...
    }
}
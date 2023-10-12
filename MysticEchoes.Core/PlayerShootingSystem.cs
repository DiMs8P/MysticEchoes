using MysticEchoes.Core.Base;

namespace MysticEchoes.Core;

public class PlayerShootingSystem : ISystem
{
    public void Update(Entity entity)
    {
        throw new NotImplementedException();

        var weaponComponent = entity.GetComponent<WeaponComponent>();
    }
}
using MysticEchoes.Core.Base;

namespace MysticEchoes.Core;

public class WeaponComponent : IComponent
{
    public Weapon CurrentWeapon { get; set; }

    public void SwitchWeapon(Weapon newWeapon)
    {
        if (newWeapon != null)
        {
            CurrentWeapon = newWeapon;
        }
    }
}
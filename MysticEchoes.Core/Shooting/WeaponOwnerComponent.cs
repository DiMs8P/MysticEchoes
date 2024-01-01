namespace MysticEchoes.Core.Shooting;

public struct WeaponOwnerComponent
{
    public List<int> WeaponIds { get; set; }
    public bool IsShooting { get; set; }

    public WeaponOwnerComponent()
    {
        WeaponIds = new List<int>();
        IsShooting = false;
    }
}
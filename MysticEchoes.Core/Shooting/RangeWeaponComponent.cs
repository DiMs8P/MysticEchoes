namespace MysticEchoes.Core.Shooting;

public struct RangeWeaponComponent
{
    public List<int> MuzzleIds { get; set; }
    public bool IsShooting { get; set; }

    public RangeWeaponComponent()
    {
        MuzzleIds = new List<int>();
        IsShooting = false;
    }
}
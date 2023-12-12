namespace MysticEchoes.Core.Shooting;

public struct WeaponComponent
{
    public WeaponType Type;
    public float TimeBetweenShoots;
    public float ElapsedTimeFromLastShoot;

    public WeaponComponent()
    {
        Type = WeaponType.None;
        TimeBetweenShoots = 0.0f;
        ElapsedTimeFromLastShoot = 0.0f;
    }
}
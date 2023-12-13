namespace MysticEchoes.Core.Shooting;

public struct WeaponComponent
{
    public WeaponType Type;
    public WeaponState State;
    public float TimeBetweenShots;
    public float ElapsedTimeFromLastShoot;

    public WeaponComponent()
    {
        State = WeaponState.None;
        Type = WeaponType.None;
        TimeBetweenShots = 0.0f;
        ElapsedTimeFromLastShoot = 0.0f;
    }
}
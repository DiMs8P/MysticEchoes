namespace MysticEchoes.Core.Shooting;

public struct MuzzleComponent
{
    public float TimeBetweenShots { get; set; }
    public float ElapsedTimeFromLastShot { get; set; }
    public bool CanFire { get; set; }
    public ShootingType ShootingType { get; set; }

    public MuzzleComponent()
    {
        TimeBetweenShots = 0.0f;
        ElapsedTimeFromLastShot = 0.0f;
        CanFire = true;
        ShootingType = ShootingType.None;
    }
}
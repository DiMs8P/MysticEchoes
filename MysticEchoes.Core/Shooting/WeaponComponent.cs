namespace MysticEchoes.Core.Shooting;

public struct WeaponComponent
{
    public WeaponType Type { get; set; }
    public WeaponState State { get; set; }
    public float TimeBetweenShots { get; set; }
    public float ElapsedTimeFromLastShoot { get; set; }
    public string ProjectilePrefabId { get; set; }
    
    public WeaponComponent()
    {
        State = WeaponState.None;
        Type = WeaponType.None;
        TimeBetweenShots = 0.0f;
        ElapsedTimeFromLastShoot = 0.0f;
        ProjectilePrefabId = "";
    }
}
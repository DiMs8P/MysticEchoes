namespace MysticEchoes.Core.Shooting;

public struct BurstFireComponent
{
    public int MaxShotsInBurst { get; set; }
    public int FiredShots { get; set; }
    public float TimeBetweenBurstShots { get; set; }

    public BurstFireComponent()
    {
        MaxShotsInBurst = 0;
        FiredShots = 0;
        TimeBetweenBurstShots = 0.0f;
    }
}
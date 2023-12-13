namespace MysticEchoes.Core.Shooting;

public struct BurstFireComponent
{
    public int MaxShotsInBurst;
    public int FiredShots;
    public float TimeBetweenBursts;
    public float TimeBetweenBurstShots;

    public BurstFireComponent()
    {
        MaxShotsInBurst = 0;
        FiredShots = 0;
        TimeBetweenBursts = 0.0f;
        TimeBetweenBurstShots = 0.0f;
    }
}
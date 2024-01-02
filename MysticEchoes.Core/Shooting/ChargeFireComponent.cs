namespace MysticEchoes.Core.Shooting;

public struct ChargeFireComponent
{
    public float MaxChargeTime { get; set; }
    public float MinChargeTime { get; set; }
    public float CurrentChargeTime { get; set; }

    public ChargeFireComponent()
    {
        MaxChargeTime = 0.0f;
        MinChargeTime = 1.0f;
        CurrentChargeTime = 0.0f;
    }
}
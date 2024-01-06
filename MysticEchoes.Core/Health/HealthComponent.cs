namespace MysticEchoes.Core.Health;

public struct HealthComponent
{
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public bool Immortal { get; set; }

    public HealthComponent()
    {
        Health = 0.0f;
        MaxHealth = 0.0f;
        Immortal = false;
    }
}
namespace MysticEchoes.Core.Health;

public struct HealthComponent
{
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public bool Immortal { get; set; }
    
    public delegate void OnPreDead(int entityId);
    public event OnPreDead OnPreDeadEvent;

    public HealthComponent()
    {
        Health = 0.0f;
        MaxHealth = 0.0f;
        Immortal = false;
    }
    
    public void InvokeOnPreDead(int entityId)
    {
        OnPreDead handler = OnPreDeadEvent;
        handler?.Invoke(entityId);
    }
}
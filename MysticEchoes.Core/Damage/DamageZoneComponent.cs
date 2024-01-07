namespace MysticEchoes.Core.Damage;

public struct DamageZoneComponent
{
    public HashSet<int> EntityToDamage { get; set; }
    public HashSet<int> DamagedEntities { get; set; }

    public DamageZoneComponent()
    {
        EntityToDamage = new HashSet<int>();
        DamagedEntities = new HashSet<int>();
    }
}
namespace MysticEchoes.Core.Shooting;

public struct OwningByComponent
{
    public int Owner { get; set; }

    public OwningByComponent()
    {
        Owner = -1;
    }
}
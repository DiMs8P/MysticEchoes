namespace MysticEchoes.Core.Shooting;

public struct OwnerComponent
{
    public List<int> OwningEntityIds { get; set; }

    public OwnerComponent()
    {
        OwningEntityIds = new List<int>();
    }
}
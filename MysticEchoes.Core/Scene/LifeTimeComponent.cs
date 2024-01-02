namespace MysticEchoes.Core.Scene;

public struct LifeTimeComponent
{
    public float LifeTime { get; set; }
    public bool IsActive { get; set; }

    public LifeTimeComponent()
    {
        LifeTime = 0;
        IsActive = true;
    }
}
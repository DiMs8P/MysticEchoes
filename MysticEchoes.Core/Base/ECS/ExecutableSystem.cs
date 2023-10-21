namespace MysticEchoes.Core.Base.ECS;

public abstract class ExecutableSystem
{
    protected readonly World World;

    protected ExecutableSystem(World world)
    {
        World = world;
    }

    public abstract void Execute();
}
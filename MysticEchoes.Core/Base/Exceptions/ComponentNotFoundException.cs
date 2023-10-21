namespace MysticEchoes.Core.Base.Exceptions;

public class ComponentNotFoundException : Exception
{
    public string ComponentName { get; }
    public override string Message => ComponentName + " wasn't found";

    public ComponentNotFoundException(string componentName)
    {
        ComponentName = componentName;
    }
}
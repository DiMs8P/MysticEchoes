using MysticEchoes.Core.Base.Exceptions;
using MysticEchoes.Core.Rendering;

namespace MysticEchoes.Core.Base;

public sealed class Entity
{
    public int Id { get; set; }
    public string? Tag { get; set; }
    public Dictionary<Type, IComponent> Components { get; }
    public RenderingType? RenderStrategy { get; set; }
    
    public Entity()
    {
        Components = new ();
    }

    public void AddComponent<T>(T component) where T : IComponent
    {
        Components[typeof(T)] = component;
    }

    public T GetComponent<T>() where T : IComponent
    {
        if (Components.TryGetValue(typeof(T), out var component))
        {
            return (T)component;
        }

        throw new ComponentNotFoundException(nameof(T));
    }

    public T? TryGetComponent<T>() where T : IComponent
    {
        if (Components.TryGetValue(typeof(T), out var component))
        {
            return (T)component;
        }

        return default;
    }
}
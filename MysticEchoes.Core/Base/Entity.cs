using MysticEchoes.Core.Base.Exceptions;

namespace MysticEchoes.Core.Base;

public class Entity
{
    public int Id { get; }
    public string? Tag { get; }
    public Dictionary<Type, IComponent> Components { get; }

    public Entity(int id, string? tag=null)
    {
        Id = id;
        Tag = tag;
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
}
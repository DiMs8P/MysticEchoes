namespace MysticEchoes.Core.Base.ECS;

public class ComponentPool<T>
    where T : Component
{
    private readonly Dictionary<int, T> _components = new();

    public T Get(int ownerId)
    {
        if (_components.TryGetValue(ownerId, out var result))
        {
            return result;
        }

        throw new Exception($"No component by ownerId {ownerId}");
    }

    public void Add(T component)
    {
        if (_components.ContainsKey(component.OwnerId))
        {
            throw new Exception($"Component {nameof(T)} with id {component.OwnerId} already exist");
        }
        _components.Add(component.OwnerId, component);
    }

    public IEnumerable<T> Enumerate()
    {
        return _components.Select(x => x.Value);
    }
}
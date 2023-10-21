namespace MysticEchoes.Core.Base.ECS;

public class World
{
    private readonly Dictionary<Type, object> _componentPools = new();

    private readonly Dictionary<int, Entity> _entities = new();

    public void RegisterComponentType<T>()
        where T : Component
    {
        _componentPools.Add(typeof(T), new ComponentPool<T>());
    }

    public ComponentPool<T> GetAllComponents<T>()
        where T : Component
    {
        return (ComponentPool<T>)_componentPools[typeof(T)];
    }

    public T GetComponent<T>(int ownerId)
        where T : Component
    {
        var pool = GetAllComponents<T>();
        return pool.Get(ownerId);
    }

    public void AddComponent<T>(T component)
        where T : Component
    {
        var pool = GetAllComponents<T>();

        if (component.OwnerId == 0)
            throw new InvalidOperationException("Компонент не привязан к сущности");

        pool.Add(component);
    }

    public void AddEntity(Entity entity)
    {
        if (_entities.ContainsKey(entity.Id))
            throw new InvalidOperationException();

        _entities.Add(entity.Id, entity);
    }

    public Entity GetEntity(int id)
    {
        return _entities[id];
    }

    public IEnumerable<Entity> GetAllComponents()
    {
        return _entities.Select(x => x.Value);
    }
}
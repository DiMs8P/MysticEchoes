namespace MysticEchoes.Core.Base;

public class EntityFactory
{
    private int _lastEntityId = 0;
    private readonly EntityPool _entities;

    public EntityFactory(EntityPool entities)
    {
        _entities = entities;
    }

    public T Create<T>(string? tag=null)
        where T : Entity, new()
    {
        _lastEntityId++;
        var entity = new T
        {
            Id = _lastEntityId,
            Tag = tag
        };

        _entities.Add(entity);

        return entity;
    }
}
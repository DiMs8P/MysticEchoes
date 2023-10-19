namespace MysticEchoes.Core.Base;

public class EntityFactory
{
    private int _lastEntityId = 0;
    private readonly EntityPool _entities;

    public EntityFactory(EntityPool entities)
    {
        _entities = entities;
    }

    public Entity Create(string? tag=null)
    {
        _lastEntityId++;
        var entity = new Entity
        {
            Id = _lastEntityId,
            Tag = tag
        };

        _entities.Add(entity);

        return entity;
    }
}
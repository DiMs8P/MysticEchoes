namespace MysticEchoes.Core.Base.ECS;

public class EntityFactory
{
    private int _lastEntityId = 1;
    private readonly World _world;

    public EntityFactory(World world)
    {
        _world = world;
    }

    public Entity Create(string? name = null)
    {
        _lastEntityId++;

        var entity = new Entity(_lastEntityId, name, _world);

        _world.AddEntity(entity);

        return entity;
    }
}
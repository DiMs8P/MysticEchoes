using MysticEchoes.Core.Rendering;

namespace MysticEchoes.Core.Base.ECS;

public class Entity
{
    private readonly World _world;

    public int Id { get; set; }
    public string? Name { get; }
    public RenderingType RenderingType { get; set; }

    public Entity(int id, string? name, World world)
    {
        Id = id;
        _world = world;
        Name = name;
    }

    public T GetComponent<T>()
        where T : Component
    {
        return _world.GetComponent<T>(Id);
    }

    public Entity AddComponent<T>(T component)
        where T : Component
    {
        component.OwnerId = Id;
        _world.AddComponent(component);

        return this;
    }
}
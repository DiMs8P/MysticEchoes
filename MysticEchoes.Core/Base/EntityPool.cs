using System.Collections;

namespace MysticEchoes.Core.Base;

public class EntityPool : IEnumerable<Entity>
{
    private readonly List<Entity> _entities = new();

    public void Add(Entity entity)
    {
        _entities.Add(entity);
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        return _entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
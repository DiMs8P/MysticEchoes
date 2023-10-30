using MysticEchoes.Core.Base.ECS;

namespace MysticEchoes.Core.Movement;

public class TransformSystem : ExecutableSystem
{
    private readonly ComponentPool<TransformComponent> _transforms;
    
    public TransformSystem(World world) 
        : base(world)
    {
        _transforms = World.GetAllComponents<TransformComponent>();
    }

    public override void Execute(SystemExecutionContext context)
    {
        foreach (var transform in _transforms.Enumerate())
        {
            transform.Position += transform.Velocity * context.DeltaTime;
        }
    }
}
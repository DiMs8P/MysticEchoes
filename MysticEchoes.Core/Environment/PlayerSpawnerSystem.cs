using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Character.Player;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Environment;

public class PlayerSpawnerSystem : IEcsInitSystem
{
    [EcsInject] private EntityFactory _factory;
    
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        int player = CreatePlayer(world, _factory);
    }

    private int CreatePlayer(EcsWorld world, EntityFactory factory)
    {
        int player = factory.Create()
            .Add(new PlayerMarker())
            .Add(new TransformComponent()
            {
                // TODO spawn player at random room
                Location = new Vector2(0.5f, 0.5f)
            })
            .Add(new MovementComponent()
            {
                Speed = 1.0f,
            })
            .Add(new RenderComponent(RenderingType.Character))
            .End();

        return player;
    }
}
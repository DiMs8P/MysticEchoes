using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Player;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class PlayerSpawnerSystem : IEcsInitSystem
{
    [EcsInject] private EntityFactory _factory;
    [EcsInject] private SystemExecutionContext _systemExecutionContext;

    private PlayerSettings _playersettings;
    public void Init(IEcsSystems systems)
    {
        _playersettings = _systemExecutionContext.Settings.PlayerSettings;
        
        CreatePlayer(_factory);
    }

    private int CreatePlayer(EntityFactory factory)
    {
        BurstFireComponent burstFireComponent = new BurstFireComponent()
        {
            MaxShotsInBurst = 10,
            TimeBetweenBursts = 1.0f,
            TimeBetweenBurstShots = 0.2f
        };
        
        int player = factory.Create()
            .Add(new PlayerMarker())
            .Add(new TransformComponent()
            {
                // TODO spawn player at random room
                Location = new Vector2(0.5f, 0.5f),
                Rotation = Vector2.UnitY
            })
            .Add(new MovementComponent()
            {
                Speed = _playersettings.Speed,
            })
            .Add(new WeaponComponent()
            {
                Type = WeaponType.TwoShoot,
                TimeBetweenShots = burstFireComponent.TimeBetweenBurstShots,
            })
            .Add(burstFireComponent)
            .Add(new RenderComponent(RenderingType.Character))
            .End();

        return player;
    }
}
using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class PlayerSpawnerSystem : IEcsInitSystem
{
    [EcsInject] private SystemExecutionContext _systemExecutionContext;
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private EntityFactory _factory;

    private PlayerSettings _playersettings;
    private EcsPool<DynamicCollider> _colliders;

    public void Init(IEcsSystems systems)
    {
        _playersettings = _systemExecutionContext.Settings.PlayerSettings;

        var world = systems.GetWorld();
        _colliders = world.GetPool<DynamicCollider>();

        CreatePlayer(_factory);
    }

    private int CreatePlayer(EntityFactory factory)
    {
        BurstFireComponent burstFireComponent = new BurstFireComponent()
        {
            MaxShotsInBurst = 3,
            TimeBetweenBursts = 2.0f,
            TimeBetweenBurstShots = 0.1f
        };

        int player = _prefabManager.CreateEntityFromPrefab(_factory, PrefabType.Player);
        ref var playerCollider = ref _colliders.Get(player);

        playerCollider.Box = new Box(player, new Rectangle(
            new Vector2(-0.2f, -0.2f),
            new Vector2(0.4f, 0.4f)
        ));


        return player;
    }
}
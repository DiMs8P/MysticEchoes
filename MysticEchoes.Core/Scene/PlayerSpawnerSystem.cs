using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class PlayerSpawnerSystem : IEcsInitSystem
{
    [EcsInject] private EntityFactory _factory;
    [EcsInject] private SystemExecutionContext _systemExecutionContext;
    [EcsInject] private PrefabManager _prefabManager;

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
            MaxShotsInBurst = 3,
            TimeBetweenBursts = 2.0f,
            TimeBetweenBurstShots = 0.1f
        };

        int player = _prefabManager.CreateEntityFromPrefab(_factory, "Player");

        return player;
    }
}
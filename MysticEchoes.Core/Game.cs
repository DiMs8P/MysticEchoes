using System.Diagnostics;
using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;
using SharpGL;
using Environment = MysticEchoes.Core.Configuration.Environment;

namespace MysticEchoes.Core;

public class Game
{
    private readonly EcsWorld _world;

    private readonly EcsSystems _setupSystems;
    private readonly EcsSystems _inputSystems;
    private readonly EcsSystems _gameplaySystems;
    private readonly EcsSystems _renderSystems;
    
    private readonly EntityFactory _entityFactory;
    private readonly SystemExecutionContext _systemExecutionContext;
    private readonly Stopwatch _updateTimer;

    public readonly Environment Environment;
    public readonly Settings GameSettings;
    
    public Game(Environment gameEnvironment, Settings gameSettings)
    {
        Environment = gameEnvironment;
        GameSettings = gameSettings;

        _world = new EcsWorld();
        _entityFactory = new EntityFactory(_world);
        
        _systemExecutionContext = new SystemExecutionContext();
        _updateTimer = new Stopwatch();
        
        _setupSystems = new EcsSystems(_world);
        _setupSystems
            .Add(new InitEnvironmentSystem())
            .Add(new PlayerSpawnerSystem())
            .Inject(_entityFactory, gameEnvironment.MazeGenerator)
            .Init();

        _inputSystems = new EcsSystems(_world);
        _inputSystems
            .Add(new PlayerMovementSystem())
            .Add(new PlayerShootingSystem())
            .Inject(gameEnvironment.InputManager)
            .Init();
        
        _gameplaySystems = new EcsSystems(_world);
        _gameplaySystems
            .Add(new WeaponShootingSystem())
            .Add(new TransformSystem())
            .Inject(_systemExecutionContext, _entityFactory)
            .Init();

        _renderSystems = new EcsSystems(_world);
        _renderSystems
            .Add(new RenderSystem())            
            .Inject(gameEnvironment.OpenGl)
            .Init();
    }

    public void Update()
    {
        // _updateTimer.Stop();
        _systemExecutionContext.DeltaTime = _updateTimer.ElapsedMilliseconds / 1e3f;
        _updateTimer.Restart();
        
        _inputSystems.Run();
        _gameplaySystems.Run();


        // _updateTimer.Start();
    }

    public void Render()
    {
        _renderSystems.Run();
    }
}
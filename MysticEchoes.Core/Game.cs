using System.Diagnostics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Assets;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;
using SharpGL;

namespace MysticEchoes.Core;

public class Game
{
    private readonly EcsWorld _world;

    private readonly EcsSystems _setupSystems;
    private readonly EcsSystems _inputSystems;
    private readonly EcsSystems _gameplaySystems;
    private EcsSystems _renderSystems;
    
    private readonly AssetManager _assetManager;
    public  readonly IInputManager _inputManager;
    private readonly IMazeGenerator _mazeGenerator;
    private readonly SystemExecutionContext _systemExecutionContext;

    private readonly EntityFactory _entityFactory;
    private readonly Stopwatch _updateTimer;


    //TODO inject settings in systems
    public Game(AssetManager assetManager, IInputManager inputManager, IMazeGenerator mazeGenerator, SystemExecutionContext systemExecutionContext)
    {
        _mazeGenerator = mazeGenerator;
        _inputManager = inputManager;
        _assetManager = assetManager;
        _systemExecutionContext = systemExecutionContext;
            
        _world = new EcsWorld();
        _entityFactory = new EntityFactory(_world);
        
        _updateTimer = new Stopwatch();
        
        _setupSystems = new EcsSystems(_world);
        _setupSystems
            .Add(new InitEnvironmentSystem())
            .Add(new PlayerSpawnerSystem())
            .Inject(_entityFactory, _mazeGenerator)
            .Init();

        _inputSystems = new EcsSystems(_world);
        _inputSystems
            .Add(new PlayerMovementSystem())
            .Add(new PlayerShootingSystem())
            .Inject(inputManager)
            .Init();
        
        _gameplaySystems = new EcsSystems(_world);
        _gameplaySystems
            .Add(new WeaponShootingSystem())
            .Add(new TransformSystem())
            .Add(new ProjectileCleanupSystem())
            .Inject(_systemExecutionContext, _entityFactory)
            .Init();
    }

    public void InitializeRender(OpenGL gl)
    {
        _assetManager.InitializeGl(gl);
        
        _renderSystems = new EcsSystems(_world);
        _renderSystems
            .Add(new RenderSystem())            
            .Inject(gl)
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
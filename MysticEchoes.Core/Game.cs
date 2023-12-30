using System.Diagnostics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Loaders;
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
    public  readonly IInputManager InputManager;

    private readonly EcsWorld _world;

    private readonly EcsSystems _setupSystems;
    private readonly EcsSystems _inputSystems;
    private readonly EcsSystems _shootingSystems;
    private readonly EcsSystems _gameplaySystems;
    private readonly EcsSystems _animationSystems;
    private readonly EcsSystems _cleanupSystems;
    private readonly EcsSystems _collisionSystems;
    private EcsSystems _renderSystems;
    
    private readonly AssetManager _assetManager;
    private readonly PrefabManager _prefabManager;
    private readonly IMazeGenerator _mazeGenerator;
    private readonly SystemExecutionContext _systemExecutionContext;

    private readonly EntityFactory _entityFactory;
    private readonly Stopwatch _updateTimer;

    //TODO inject settings in systems
    public Game(
        AssetManager assetManager, 
        PrefabManager prefabManager, 
        IInputManager inputManager, 
        IMazeGenerator mazeGenerator, 
        SystemExecutionContext systemExecutionContext
        )
    {
        _mazeGenerator = mazeGenerator;
        InputManager = inputManager;
        _assetManager = assetManager;
        _prefabManager = prefabManager;
        _systemExecutionContext = systemExecutionContext;
            
        _world = new EcsWorld();
        _entityFactory = new EntityFactory(_world);
        
        _updateTimer = new Stopwatch();
        
        _setupSystems = new EcsSystems(_world);
        _setupSystems
            .Add(new InitEnvironmentSystem())
            .Add(new PlayerSpawnerSystem())
            .Inject(_entityFactory, _prefabManager, _mazeGenerator)
            .Init();

        _inputSystems = new EcsSystems(_world);
        _inputSystems
            .Add(new PlayerMovementSystem())
            .Add(new PlayerShootingSystem())
            .Add(new PlayerAnimationSystem())
            .Inject(inputManager, _systemExecutionContext)
            .Init();

        _shootingSystems = new EcsSystems(_world);
        _shootingSystems
            .Add(new WeaponsStateSystem())
            .Add(new BurstFireSystem())
            .Add(new WeaponShootingSystem())
            .Inject(_systemExecutionContext, _entityFactory, _prefabManager)
            .Init();

        _collisionSystems = new EcsSystems(_world);
        _collisionSystems
            .Add(new CollisionsSystem())
            .Inject(_entityFactory)
            .Init();

        _gameplaySystems = new EcsSystems(_world);
        _gameplaySystems
            .Add(new TransformSystem())
            .Inject(_systemExecutionContext)
            .Init();

        _animationSystems = new EcsSystems(_world);
        _animationSystems
            .Add(new AnimationSystem())
            .Inject(_systemExecutionContext)
            .Init();

        _cleanupSystems = new EcsSystems(_world);
        _cleanupSystems
            .Add(new ProjectileCleanupSystem())
            .Init();
    }

    public void InitializeRender(OpenGL gl)
    {
        _assetManager.InitializeGl(gl);
        
        _renderSystems = new EcsSystems(_world);
        _renderSystems
            .Add(new RenderSystem())
            .Inject(gl, _assetManager)
            .Init();
    }
    
    public void Update()
    {
        // _updateTimer.Stop();
        _systemExecutionContext.DeltaTime = _updateTimer.ElapsedMilliseconds / 1e3f;
        _updateTimer.Restart();
        
        _inputSystems.Run();
        _collisionSystems.Run();
        _shootingSystems.Run();
        _gameplaySystems.Run();
        _animationSystems.Run();
        _cleanupSystems.Run();

        // _updateTimer.Start();
    }

    public void Render()
    {
        _renderSystems.Run();
    }
}
using System.Diagnostics;
using Leopotam.EcsLite;
using MysticEchoes.Core.AI;
using MysticEchoes.Core.AI.Factories;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Control;
using MysticEchoes.Core.Items;
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
    private readonly EcsSystems _controlsSystems;
    private readonly EcsSystems _shootingSystems;
    private readonly EcsSystems _gameplaySystems;
    private readonly EcsSystems _animationSystems;
    private readonly EcsSystems _cleanupSystems;
    private readonly EcsSystems _collisionSystems;
    private EcsSystems _renderSystems;
    
    private readonly AnimationManager _animationManager;
    private readonly AssetManager _assetManager;
    private readonly PrefabManager _prefabManager;
    private readonly IMazeGenerator _mazeGenerator;
    private readonly SystemExecutionContext _systemExecutionContext;

    private readonly EntityBuilder _entityBuilder;
    private readonly ItemsFactory _itemsFactory;
    private readonly EnemyFactory _enemyFactory;
    
    private readonly Stopwatch _updateTimer;

    //TODO inject settings in systems
    public Game(
        AnimationManager animationManager,
        AssetManager assetManager, 
        PrefabManager prefabManager, 
        IInputManager inputManager, 
        IMazeGenerator mazeGenerator, 
        SystemExecutionContext systemExecutionContext
        )
    {
        _mazeGenerator = mazeGenerator;
        InputManager = inputManager;
        _animationManager = animationManager;
        _assetManager = assetManager;
        _prefabManager = prefabManager;
        _systemExecutionContext = systemExecutionContext;
            
        _world = new EcsWorld();
        _entityBuilder = new EntityBuilder(_world);
        _itemsFactory = new ItemsFactory(_world, _entityBuilder, _prefabManager, _systemExecutionContext.Settings.ItemsSettings);
        _enemyFactory = new EnemyFactory(_world, _entityBuilder, _prefabManager);
        
        _updateTimer = new Stopwatch();
        
        _setupSystems = new EcsSystems(_world);
        _setupSystems
            .Add(new InitEnvironmentSystem())
            .Add(new PlayerSpawnerSystem())
            .Add(new EnemySpawnerSystem())
            .Inject(_entityBuilder, _prefabManager, _itemsFactory, _enemyFactory, _animationManager, _mazeGenerator, systemExecutionContext.Settings)
            .Init();

        _controlsSystems = new EcsSystems(_world);
        _controlsSystems
            .Add(new PlayerControlSystem())
            .Add(new UnitsMovementSystem())
            .Add(new PlayerShootingSystem())
            .Inject(inputManager, _systemExecutionContext)
            .Init();

        _shootingSystems = new EcsSystems(_world);
        _shootingSystems
            .Add(new WeaponShootingSystem())
            .Inject(_systemExecutionContext, _entityBuilder, _prefabManager)
            .Init();

        _gameplaySystems = new EcsSystems(_world);
        _gameplaySystems
            .Add(new TransformSystem())
            .Add(new CollidersMovementSystem())
            .Add(new AiSystem())
            .Inject(_systemExecutionContext)
            .Init();

        _collisionSystems = new EcsSystems(_world);
        _collisionSystems
            .Add(new CollisionsSystem())
            .Inject(_entityBuilder, _systemExecutionContext, _prefabManager)
            .Init();

        _animationSystems = new EcsSystems(_world);
        _animationSystems
            .Add(new AnimationStateMachineSystem())
            .Add(new AnimationSystem())
            .Inject(_animationManager, _systemExecutionContext)
            .Init();
        
        _cleanupSystems = new EcsSystems(_world);
        _cleanupSystems
            .Add(new LifeTimeCleanupSystem())
            .Inject(_systemExecutionContext)
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
        _systemExecutionContext.FrameNumber += 1;
        _updateTimer.Restart();
        
        _controlsSystems.Run();
        _shootingSystems.Run();
        _gameplaySystems.Run();
        _collisionSystems.Run();
        _animationSystems.Run();
        _cleanupSystems.Run();

        // _updateTimer.Start();
    }

    public void Render()
    {
        _renderSystems.Run();
    }
}
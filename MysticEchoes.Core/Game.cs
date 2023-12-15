﻿using System.Diagnostics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;

namespace MysticEchoes.Core;

public class Game
{
    public readonly IInputManager InputManager;

    private readonly EcsWorld _world;

    private readonly EcsSystems _setupSystems;
    private EcsSystems _inputSystems;
    private readonly EcsSystems _shootingSystems;
    private readonly EcsSystems _gameplaySystems;
    private readonly EcsSystems _cleanupSystems;
    private EcsSystems _renderSystems;

    private readonly AssetManager _assetManager;
    private readonly PrefabManager _prefabManager;
    private readonly IMazeGenerator _mazeGenerator;
    private readonly SystemExecutionContext _systemExecutionContext;

    private readonly EntityFactory _entityFactory;
    private readonly Stopwatch _updateTimer;
    private OpenGL _gl;

    //TODO inject settings in systems
    public Game(AssetManager assetManager, PrefabManager prefabManager, IInputManager inputManager, IMazeGenerator mazeGenerator, SystemExecutionContext systemExecutionContext)
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
            .Inject(_entityFactory, _prefabManager, _mazeGenerator, _systemExecutionContext)
            .Init();

        _shootingSystems = new EcsSystems(_world);
        _shootingSystems
            .Add(new WeaponsStateSystem())
            .Add(new BurstFireSystem())
            .Add(new WeaponShootingSystem())
            .Inject(_systemExecutionContext, _entityFactory, _prefabManager)
            .Init();

        _gameplaySystems = new EcsSystems(_world);
        _gameplaySystems
            .Add(new TransformSystem())
            .Inject(_systemExecutionContext)
            .Init();

        _cleanupSystems = new EcsSystems(_world);
        _cleanupSystems
            .Add(new ProjectileCleanupSystem())
            .Init();
    }

    public void InitializeRender(OpenGL gl)
    {
        _gl = gl;
        _assetManager.InitializeGl(gl);

        _inputSystems = new EcsSystems(_world);
        _inputSystems
            .Add(new PlayerMovementSystem())
            .Add(new PlayerShootingSystem())
            .Inject(InputManager, _systemExecutionContext, gl)
            .Init();

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
        _shootingSystems.Run();
        _gameplaySystems.Run();
        _cleanupSystems.Run();

        // _updateTimer.Start();
    }

    public void Render()
    {
        _renderSystems.Run();
        _systemExecutionContext.MatrixView = _gl.GetModelViewMatrix();
        _systemExecutionContext.MatrixProjection = _gl.GetProjectionMatrix();
    }

    public void Destroy()
    {
        _world.Destroy();

        _setupSystems.Destroy();
        _inputSystems.Destroy();
        _shootingSystems.Destroy();
        _gameplaySystems.Destroy();
        _cleanupSystems.Destroy();
        _renderSystems.Destroy();
    }
}
﻿using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class InitEnvironmentSystem : IEcsInitSystem
{
    [EcsInject] private IMazeGenerator _mazeGenerator;
    [EcsInject] private EntityFactory _factory;
    [EcsInject] private PrefabManager _prefabManager;
    public void Init(IEcsSystems systems)
    {
        CreateTiles();
        //CreateSquare();
        CreateItem();
    }

    private void CreateTiles()
    {
        var map = _mazeGenerator.Generate();

        var mapEntity = _factory.Create()
            .Add(new TileMapComponent(map))
            .Add(new RenderComponent(RenderingType.TileMap))
            .End();
    }
    
    private void CreateSquare()
    {
        _factory.Create()
            .Add(new TransformComponent{
                Location = new Vector2(0, 0.3f),
                Rotation = new Vector2(1.0f, 0.0f)
            })
            .Add(new MovementComponent()
            {
                Speed = 1.0f,
                Velocity = new Vector2(0.5f, 0.5f)
            })
            .Add(new RenderComponent(RenderingType.DebugUnitView))
            .End();
    }
    
    private void CreateItem()
    {
        int entityId = _prefabManager.CreateEntityFromPrefab(_factory, PrefabType.Coin);
    }
}
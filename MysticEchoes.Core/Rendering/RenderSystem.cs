﻿using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using MazeGeneration;
using MazeGeneration.TreeModule;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;
using Rectangle = MysticEchoes.Core.Base.Geometry.Rectangle;


namespace MysticEchoes.Core.Rendering;

public class RenderSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private AssetManager _assetManager;
    [EcsInject] private OpenGL _gl;

    private EcsFilter _rendersFilter;
    private EcsPool<RenderComponent> _renders;
    private EcsPool<SpriteComponent> _sprites;

    private EcsPool<SpaceTreeComponent> _spaceTrees;

    private EcsPool<TransformComponent> _transforms;
    private EcsPool<TileMapComponent> _tileMaps;
    private EcsPool<StaticCollider> _staticColliders;
    private EcsPool<DynamicCollider> _dynamicColliders;
    private double t;

    private static readonly Dictionary<CellType, double[]> TileColors = new()
    {
        [CellType.Empty] = new[] { 64d / 255, 64d / 255, 64d / 255 },
        [CellType.FragmentBound] = new[] { 0d, 0d, 0d },
        [CellType.Hall] = new[] { 0.8d, 0.8d, 0.1d },
        [CellType.ControlPoint] = new[] { 0.8d, 0.1d, 0.1d },
        [CellType.Wall] = new[] { 103d / 255, 65d / 255, 72d / 255 },
        [CellType.Floor] = new[] { 53d / 255, 25d / 255, 48d / 255 }
    };

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _renders = world.GetPool<RenderComponent>();
        _sprites = world.GetPool<SpriteComponent>();
        _rendersFilter = world.Filter<RenderComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _tileMaps = world.GetPool<TileMapComponent>();
        _staticColliders = world.GetPool<StaticCollider>();
        _dynamicColliders = world.GetPool<DynamicCollider>();
        _spaceTrees = world.GetPool<SpaceTreeComponent>();

        _gl.Enable(OpenGL.GL_TEXTURE_2D);

        _gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
        _gl.Enable(OpenGL.GL_BLEND);
    }

    public void Run(IEcsSystems systems)
    {
        if (_gl is null)
            throw new InvalidOperationException("Open Gl wasn't initialized");

        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _gl.LoadIdentity();
        _gl.Ortho(0, 2, 0, 2, -1, 1);
        //_gl.Ortho(0, 0.8, 0, 0.5, -1, 1);
        //_gl.Translate(-(1-0.29f), -1.4, 0f);
        //t += 0.001;

        foreach (var entityId in _rendersFilter)
        {
            ref RenderComponent render = ref _renders.Get(entityId);

            if (render.Type is RenderingType.TileMap)
            {
                ref TileMapComponent map = ref _tileMaps.Get(entityId);
                {
                    var color = TileColors[CellType.Empty];
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                    _gl.Color(color[0], color[1], color[2]);
                    _gl.Vertex(0d, 0d);
                    _gl.Vertex(2d, 0d);
                    _gl.Vertex(2d, 2d);
                    _gl.Vertex(0d, 2d);
                    _gl.End();
                }

                foreach (var floor in map.Tiles.FloorTiles)
                {
                    PrintTile(floor, map, "Floor");
                }
                foreach (var door in map.Tiles.DoorTiles)
                {
                    PrintTile(door, map, "HorizontalDoor");
                }
                foreach (var wall in map.Tiles.WallTopTiles)
                {
                    PrintTile(wall, map, "WallTop");
                }
                foreach (var door in map.Tiles.WallSideRightTiles)
                {
                    PrintTile(door, map, "WallSideRight");
                }
                foreach (var door in map.Tiles.WallSideLeftTiles)
                {
                    PrintTile(door, map, "WallSideLeft");
                }
                foreach (var door in map.Tiles.WallBottomTiles)
                {
                    PrintTile(door, map, "WallBottom");
                }
                foreach (var door in map.Tiles.WallFullTiles)
                {
                    PrintTile(door, map, "WallFull");
                }
                foreach (var door in map.Tiles.WallInnerCornerDownLeft)
                {
                    PrintTile(door, map, "WallInnerCornerDownLeft");
                }
                foreach (var door in map.Tiles.WallInnerCornerDownRight)
                {
                    PrintTile(door, map, "WallInnerCornerDownRight");
                }
                foreach (var door in map.Tiles.WallDiagonalCornerDownLeft)
                {
                    PrintTile(door, map, "WallDiagonalCornerDownLeft");
                }
                foreach (var door in map.Tiles.WallDiagonalCornerDownRight)
                {
                    PrintTile(door, map, "WallDiagonalCornerDownRight");
                }
                foreach (var door in map.Tiles.WallDiagonalCornerUpLeft)
                {
                    PrintTile(door, map, "WallDiagonalCornerUpLeft");
                }
                foreach (var door in map.Tiles.WallDiagonalCornerUpRight)
                {
                    PrintTile(door, map, "WallDiagonalCornerUpRight");
                }
            }
            else if (render.Type is RenderingType.StaticColliderDebugView)
            {
                //_gl.Begin(OpenGL.GL_LINE_LOOP);
                //var collider = _staticColliders.Get(entityId);

                //var rect = collider.Box.Shape;

                //_gl.Color(1.0f, 0.3f, 0.0f);

                //_gl.Vertex(rect.Left, rect.Bottom);
                //_gl.Vertex(rect.Left, rect.Top);
                //_gl.Vertex(rect.Right, rect.Top);
                //_gl.Vertex(rect.Right, rect.Bottom);
                //_gl.End();
            }
            else if (render.Type is RenderingType.DynamicColliderDebugView)
            {
                //_gl.Begin(OpenGL.GL_LINE_LOOP);
                //var collider = _dynamicColliders.Get(entityId);

                //var rect = collider.Box.Shape;

                //_gl.Color(0.1f, 0.4f, 1.0f);

                //_gl.Vertex(rect.Left, rect.Bottom);
                //_gl.Vertex(rect.Left, rect.Top);
                //_gl.Vertex(rect.Right, rect.Top);
                //_gl.Vertex(rect.Right, rect.Bottom);
                //_gl.End();
            }
            else if (render.Type is RenderingType.ColliderSpaceTreeView)
            {
                var tree = _spaceTrees.Get(entityId).Tree;

                var stack = new Stack<QuadTree>();
                stack.Push(tree);

                while (stack.Count > 0)
                {
                    tree = stack.Pop();
                    var rect = tree.Bound;

                    _gl.Begin(OpenGL.GL_LINE_LOOP);
                    _gl.Color(1.0f, 1.0f, 1.0f, 0.2f);
                    _gl.Vertex(rect.Left, rect.Bottom);
                    _gl.Vertex(rect.Left, rect.Top);
                    _gl.Vertex(rect.Right, rect.Top);
                    _gl.Vertex(rect.Right, rect.Bottom);
                    _gl.End();

                    foreach (var subTree in tree.SubTrees)
                    {
                        stack.Push(subTree);
                    }
                }
            }
            else if (render.Type is RenderingType.DebugUnitView)
            {
                ref TransformComponent transform = ref _transforms.Get(entityId);

                _gl.Begin(OpenGL.GL_QUADS);
                _gl.Color(1f, 0f, 0f);

                const float halfSize = 0.05f;

                _gl.Vertex(transform.Location.X - halfSize, transform.Location.Y + halfSize);
                _gl.Vertex(transform.Location.X - halfSize, transform.Location.Y - halfSize);
                _gl.Vertex(transform.Location.X + halfSize, transform.Location.Y - halfSize);
                _gl.Vertex(transform.Location.X + halfSize, transform.Location.Y + halfSize);
                _gl.End();
            }
            else if (render.Type is RenderingType.Character)
            {
                ref SpriteComponent spriteComponent = ref _sprites.Get(entityId);

                _gl.PushMatrix();

                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, _assetManager.GetTexture(spriteComponent.Sprite));

                ref TransformComponent transform = ref _transforms.Get(entityId);

                _gl.Translate(transform.Location);
                _gl.Scale(transform.Scale);

                _gl.Begin(OpenGL.GL_QUADS);

                _gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

                const float halfSize = 0.2f;
                _gl.TexCoord(0.0, 0.0f);
                _gl.Vertex(-halfSize, +halfSize);
                _gl.TexCoord(0.0, 1.0f);
                _gl.Vertex(-halfSize, -halfSize);
                _gl.TexCoord(1.0, 1.0f);
                _gl.Vertex(+halfSize, -halfSize);
                _gl.TexCoord(1.0, 0.0f);
                _gl.Vertex(+halfSize, +halfSize);
                _gl.End();

                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

                _gl.PopMatrix();
                _gl.GetModelViewMatrix();

                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

                _gl.PushMatrix();

                var collider = _dynamicColliders.Get(entityId);
                var rect = collider.Box.Shape;
                _gl.Translate(rect.LeftBottom);

                _gl.Begin(OpenGL.GL_LINE_LOOP);

                _gl.Color(1.0f, 0.3f, 0.0f);

                _gl.Vertex(0, 0);
                _gl.Vertex(0, rect.Size.Y);
                _gl.Vertex(rect.Size.X, rect.Size.Y);
                _gl.Vertex(rect.Size.X, 0);
                _gl.End();

                _gl.PopMatrix();
            }
            else if (render.Type is RenderingType.General)
            {
                ref SpriteComponent spriteComponent = ref _sprites.Get(entityId);

                _gl.PushMatrix();

                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, _assetManager.GetTexture(spriteComponent.Sprite));

                ref TransformComponent transform = ref _transforms.Get(entityId);

                _gl.Translate(transform.Location);
                _gl.Rotate(transform.Rotation.GetAngleBetweenGlobalX());
                _gl.Scale(transform.Scale);

                _gl.Begin(OpenGL.GL_QUADS);

                _gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

                const float halfSize = 0.2f;
                _gl.TexCoord(0.0, 0.0f);
                _gl.Vertex(-halfSize, +halfSize);
                _gl.TexCoord(0.0, 1.0f);
                _gl.Vertex(-halfSize, -halfSize);
                _gl.TexCoord(1.0, 1.0f);
                _gl.Vertex(+halfSize, -halfSize);
                _gl.TexCoord(1.0, 0.0f);
                _gl.Vertex(+halfSize, +halfSize);
                _gl.End();

                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

                _gl.PopMatrix();
                _gl.GetModelViewMatrix();

                _gl.PushMatrix();

                var collider = _dynamicColliders.Get(entityId);
                var rect = collider.Box.Shape;
                _gl.Translate(rect.LeftBottom);

                _gl.Begin(OpenGL.GL_LINE_LOOP);

                _gl.Color(1.0f, 0.3f, 0.0f);

                _gl.Vertex(0, 0);
                _gl.Vertex(0, rect.Size.Y);
                _gl.Vertex(rect.Size.X, rect.Size.Y);
                _gl.Vertex(rect.Size.X, 0);
                _gl.End();

                _gl.PopMatrix();

            }
            else if (render.Type is not RenderingType.None)
            {
                throw new NotImplementedException();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PrintTile(Point position, TileMapComponent map, string texture)
    {
        _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
        _gl.BindTexture(OpenGL.GL_TEXTURE_2D, _assetManager.GetTexture(texture));

        _gl.Begin(OpenGL.GL_TRIANGLE_FAN);

        var rect = new Rectangle(
            new Vector2(position.X * map.TileSize.X, position.Y * map.TileSize.Y),
            map.TileSize
        );

        _gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

        const float p = 9e-2f;

        _gl.TexCoord(0.0 + p, 0.0f + p);
        _gl.Vertex(rect.Left, rect.Top);
        _gl.TexCoord(0.0+ p, 1.0f - p);
        _gl.Vertex(rect.Left, rect.Bottom);
        _gl.TexCoord(1.0 - p, 1.0f - p);
        _gl.Vertex(rect.Right, rect.Bottom);
        _gl.TexCoord(1.0 - p, 0.0f + p);
        _gl.Vertex(rect.Right, rect.Top);
        _gl.End();

        _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
        _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
    }
}
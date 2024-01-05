using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using MazeGeneration;
using MazeGeneration.Enemies;
using MazeGeneration.TreeModule;
using MazeGeneration.TreeModule.Rooms;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.MapModule.Rooms;
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
    private EcsPool<EnemySpawnComponent> _enemySpawns;
    private EcsPool<DoorComponent> _doors;

    private double t;

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
        _enemySpawns = world.GetPool<EnemySpawnComponent>();
        _doors = world.GetPool<DoorComponent>();

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
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                    _gl.Color(64f / 255, 64f / 255, 64f / 255);
                    _gl.Vertex(0d, 0d);
                    _gl.Vertex(2d, 0d);
                    _gl.Vertex(2d, 2d);
                    _gl.Vertex(0d, 2d);
                    _gl.End();
                }

                foreach (var floor in map.Map.FloorTiles)
                {
                    PrintTile(floor, map, "Floor");
                }
                foreach (var wall in map.Map.WallTopTiles)
                {
                    PrintTile(wall, map, "WallTop");
                }
                foreach (var wall in map.Map.WallSideRightTiles)
                {
                    PrintTile(wall, map, "WallSideRight");
                }
                foreach (var wall in map.Map.WallSideLeftTiles)
                {
                    PrintTile(wall, map, "WallSideLeft");
                }
                foreach (var wall in map.Map.WallBottomTiles)
                {
                    PrintTile(wall, map, "WallBottom");
                }
                foreach (var wall in map.Map.WallFullTiles)
                {
                    PrintTile(wall, map, "WallFull");
                }
                foreach (var wall in map.Map.WallInnerCornerDownLeft)
                {
                    PrintTile(wall, map, "WallInnerCornerDownLeft");
                }
                foreach (var wall in map.Map.WallInnerCornerDownRight)
                {
                    PrintTile(wall, map, "WallInnerCornerDownRight");
                }
                foreach (var wall in map.Map.WallDiagonalCornerDownLeft)
                {
                    PrintTile(wall, map, "WallDiagonalCornerDownLeft");
                }
                foreach (var wall in map.Map.WallDiagonalCornerDownRight)
                {
                    PrintTile(wall, map, "WallDiagonalCornerDownRight");
                }
                foreach (var wall in map.Map.WallDiagonalCornerUpLeft)
                {
                    PrintTile(wall, map, "WallDiagonalCornerUpLeft");
                }
                foreach (var wall in map.Map.WallDiagonalCornerUpRight)
                {
                    PrintTile(wall, map, "WallDiagonalCornerUpRight");
                }
            }
            else if (render.Type is RenderingType.StaticColliderDebugView)
            {
                //var collider = _staticColliders.Get(entityId);

                //var rect = collider.Box.Shape;
                //_gl.Begin(OpenGL.GL_LINE_LOOP);
                //_gl.Color(1.0f, 0.3f, 0.0f);

                //_gl.Vertex(rect.Left, rect.Bottom);
                //_gl.Vertex(rect.Left, rect.Top);
                //_gl.Vertex(rect.Right, rect.Top);
                //_gl.Vertex(rect.Right, rect.Bottom);
                //_gl.End();
            }
            else if (render.Type is RenderingType.DynamicColliderDebugView)
            {
                var collider = _dynamicColliders.Get(entityId);
                var rect = collider.Box.Shape;

                DrawCollider(rect, 1.0f, 0.7f, 0.1f);
            }
            else if (render.Type is RenderingType.EntranceTrigger)
            {
                var collider = _dynamicColliders.Get(entityId);
                var rect = collider.Box.Shape;

                DrawCollider(rect, 0.1f, 0.3f, 1.0f);
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
                var layer = 0f;

                _gl.TexCoord(0.0, 0.0f);
                _gl.Vertex(-halfSize, +halfSize, layer);
                _gl.TexCoord(0.0, 1.0f);
                _gl.Vertex(-halfSize, -halfSize, layer);
                _gl.TexCoord(1.0, 1.0f);
                _gl.Vertex(+halfSize, -halfSize, layer);
                _gl.TexCoord(1.0, 0.0f);
                _gl.Vertex(+halfSize, +halfSize, layer);
                _gl.End();

                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

                _gl.PopMatrix();
                _gl.GetModelViewMatrix();

                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

                _gl.PushMatrix();

                var collider = _dynamicColliders.Get(entityId);
                DrawCollider(collider.Box.Shape, 1.0f, 0.3f, 0.0f);
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
            else if (render.Type is RenderingType.EnemySpawn)
            {
                void ChooseSpawnColor(EnemySpawnComponent enemySpawnComponent, float alpha = 1f)
                {
                    if (enemySpawnComponent.Data.Type is EnemyType.Common)
                    {
                        _gl.Color(0.0f, 1.0f, 0.0f, alpha);
                    }
                    else if (enemySpawnComponent.Data.Type is EnemyType.Elite)
                    {
                        _gl.Color(1.0f, 1.0f, 0.0f, alpha);
                    }
                    else if (enemySpawnComponent.Data.Type is EnemyType.MiniBoss)
                    {
                        _gl.Color(1.0f, 0.0f, 0.0f, alpha);
                    }
                }

                var collider = _dynamicColliders.Get(entityId);
                var spawn = _enemySpawns.Get(entityId);

                var rect = collider.Box.Shape;

                _gl.Begin(OpenGL.GL_LINE_LOOP);

                ChooseSpawnColor(spawn);

                _gl.Vertex(rect.Left, rect.Bottom);
                _gl.Vertex(rect.Left, rect.Top);
                _gl.Vertex(rect.Right, rect.Top);
                _gl.Vertex(rect.Right, rect.Bottom);
                _gl.End();

                _gl.Begin(OpenGL.GL_QUADS);

                ChooseSpawnColor(spawn, 0.2f);

                _gl.Vertex(rect.Left, rect.Bottom);
                _gl.Vertex(rect.Left, rect.Top);
                _gl.Vertex(rect.Right, rect.Top);
                _gl.Vertex(rect.Right, rect.Bottom);
                _gl.End();
            }
            else if (render.Type is RenderingType.Door)
            {
                ref var map = ref _tileMaps.Get(entityId);
                ref var door = ref _doors.Get(entityId);

                #region textureSelection
                var texture = door.IsOpen switch
                {
                    true => door.Orientation switch
                    {
                        DoorOrientation.Horizontal => "HorizontalDoorOpen",
                        DoorOrientation.VerticalLeft => "DoorLeftOpen",
                        DoorOrientation.VerticalRight => "DoorRightOpen",
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    false => door.Orientation switch
                    {
                        DoorOrientation.Horizontal => "HorizontalDoorOpen",
                        DoorOrientation.VerticalLeft => "DoorLeftOpen",
                        DoorOrientation.VerticalRight => "DoorRightOpen",
                        _ => throw new ArgumentOutOfRangeException()
                    }
                };
                #endregion

                PrintTile(door.Tile, map, texture);
            }
            else if (render.Type is not RenderingType.None)
            {
                throw new NotImplementedException();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawCollider(Rectangle rect, double r, double g, double b)
    {
        _gl.Begin(OpenGL.GL_LINE_LOOP);
        _gl.Color(r, g, b);


        _gl.Vertex(rect.Left, rect.Bottom);
        _gl.Vertex(rect.Left, rect.Top);
        _gl.Vertex(rect.Right, rect.Top);
        _gl.Vertex(rect.Right, rect.Bottom);
        _gl.End();
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
        _gl.TexCoord(0.0 + p, 1.0f - p);
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
using System.Numerics;
using Leopotam.EcsLite;
using MazeGeneration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Assets;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;
using Point = MysticEchoes.Core.Base.Geometry.Point;
using Rectangle = MysticEchoes.Core.Base.Geometry.Rectangle;
using Size = MysticEchoes.Core.Base.Geometry.Size;


namespace MysticEchoes.Core.Rendering;

public class RenderSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private AssetManager _assetManager;
    [EcsInject] private OpenGL _gl;

    private EcsFilter _rendersFilter;
    private EcsPool<RenderComponent> _renders;
    
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<TileMapComponent> _tileMaps;

    private static readonly Dictionary<CellType, double[]> TileColors = new()
    {
        [CellType.Empty] = new[] { 64d/255, 64d/255, 64d/255 },
        [CellType.FragmentBound] = new[] { 0d,0d,0d },
        [CellType.Hall] = new[] { 0.8d, 0.8d, 0.1d },
        [CellType.ControlPoint] = new[] { 0.8d, 0.1d, 0.1d },
        [CellType.Wall] = new[] { 103d/255, 65d/255, 72d/255 },
        [CellType.Floor] = new[] {53d/255,25d/255,48d/255}
    };

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        _renders = world.GetPool<RenderComponent>();
        _rendersFilter = world.Filter<RenderComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _tileMaps = world.GetPool<TileMapComponent>();
        
        _gl.Enable(OpenGL.GL_TEXTURE_2D);
        
                
        _gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
        _gl.Enable( OpenGL.GL_BLEND );
    }

    public void Run(IEcsSystems systems)
    {
        if (_gl is null)
            throw new InvalidOperationException("Open Gl wasn't initialized");

        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _gl.LoadIdentity();
        _gl.Ortho(0, 2, 0, 2, -1, 1);

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
                    _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                    _gl.BindTexture(OpenGL.GL_TEXTURE_2D, _assetManager.GetTexture(AssetType.Floor));
                    
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                
                    var rect = new Rectangle(
                        new Point(floor.X * map.TileSize.Width, floor.Y * map.TileSize.Height),
                        new Size(map.TileSize.Width, map.TileSize.Height)
                    );
                    
                    _gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
                
                    _gl.TexCoord(0.0, 0.0f);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
                    _gl.TexCoord(0.0, 1.0f);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.TexCoord(1.0, 1.0f);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.TexCoord(1.0, 0.0f);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
                    _gl.End();
                    
                    _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                    _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
                }
                foreach (var floor in map.Tiles.WallTiles)
                {
                    _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                    _gl.BindTexture(OpenGL.GL_TEXTURE_2D, _assetManager.GetTexture(AssetType.Wall));
                    
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);

                    var rect = new Rectangle(
                        new Point(floor.X * map.TileSize.Width, floor.Y * map.TileSize.Height),
                        new Size(map.TileSize.Width, map.TileSize.Height)
                    );

                    _gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
                    
                    _gl.TexCoord(0.0, 0.0f);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
                    _gl.TexCoord(0.0, 1.0f);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.TexCoord(1.0, 1.0f);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.TexCoord(1.0, 0.0f);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
                    _gl.End();
                    
                    _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                    _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
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
                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, _assetManager.GetTexture(AssetType.Player));
                
                ref TransformComponent transform = ref _transforms.Get(entityId);

                _gl.Begin(OpenGL.GL_QUADS);
                
                _gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

                const float halfSize = 0.2f;
                _gl.TexCoord(0.0, 0.0f);
                _gl.Vertex(transform.Location.X - halfSize, transform.Location.Y + halfSize);
                _gl.TexCoord(0.0, 1.0f);
                _gl.Vertex(transform.Location.X - halfSize, transform.Location.Y - halfSize);
                _gl.TexCoord(1.0, 1.0f);
                _gl.Vertex(transform.Location.X + halfSize, transform.Location.Y - halfSize);
                _gl.TexCoord(1.0, 0.0f);
                _gl.Vertex(transform.Location.X + halfSize, transform.Location.Y + halfSize);
                _gl.End();
                
                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            }
            else if (render.Type is RenderingType.Bullet)
            {
                _gl.PushMatrix();
                
                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, _assetManager.GetTexture(AssetType.Bullet));
                
                ref TransformComponent transform = ref _transforms.Get(entityId);
                _gl.Translate(transform.Location);
                _gl.Rotate(transform.Rotation.GetAngleBetweenGlobalX());
                _gl.Scale(transform.Scale);
                
                _gl.Begin(OpenGL.GL_QUADS);
                
                _gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

                const float halfSize = 0.2f;
                _gl.TexCoord(0.0, 0.0f);
                _gl.Vertex(- halfSize, + halfSize);
                _gl.TexCoord(0.0, 1.0f);
                _gl.Vertex(- halfSize, - halfSize);
                _gl.TexCoord(1.0, 1.0f);
                _gl.Vertex(+ halfSize, - halfSize);
                _gl.TexCoord(1.0, 0.0f);
                _gl.Vertex(+ halfSize, + halfSize);
                _gl.End();
                
                _gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                _gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
                
                _gl.PopMatrix();
                _gl.GetModelViewMatrix();
            }
            else if (render.Type is not RenderingType.None)
            {
                throw new NotImplementedException();
            }
        }
    }
}
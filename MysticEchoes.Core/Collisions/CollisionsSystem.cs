using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Collisions;

public class CollisionsSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private EntityFactory _factory;

    private int _mapId;
    private EcsFilter _staticEntities;
    private EcsFilter _dynamicEntities;
    private EcsPool<StaticCollider> _staticColliders;
    private EcsPool<DynamicCollider> _dynamicColliders;


    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        var mapFilter = world.Filter<TileMapComponent>().End();
        if (mapFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 map");
        }
        _mapId = mapFilter.GetRawEntities()[0];

        _staticEntities = world.Filter<StaticCollider>().End();
        _dynamicEntities = world.Filter<DynamicCollider>().End();
        _staticColliders = world.GetPool<StaticCollider>();
        _dynamicColliders = world.GetPool<DynamicCollider>();

        ref var map = ref world.GetPool<TileMapComponent>().Get(_mapId);

        var staticCollidersTree = new QuadTree(
            new Rectangle(
                new Vector2(0, 0), 
                new Vector2(map.Tiles.Size.Width * map.TileSize.X, map.Tiles.Size.Height * map.TileSize.Y)
            ),
            3
        );

        foreach (var staticEntity in _staticEntities)
        {
            var collider = _staticColliders.Get(staticEntity);
            staticCollidersTree.Add(collider.Box);
        }

        _factory.Create()
            .Add(new SpaceTreeComponent(){Tree = staticCollidersTree})
            .Add(new RenderComponent(RenderingType.ColliderSpaceTreeView));
    }

    public void Run(IEcsSystems systems)
    {

    }
}
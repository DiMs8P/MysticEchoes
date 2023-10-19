using MysticEchoes.Core.Base;

namespace MysticEchoes.Core.MapModule;

public class Tile : Entity
{
    public Tile() : base()
    {
        RenderStrategy = new TileRenderStrategy(this);
    }
}
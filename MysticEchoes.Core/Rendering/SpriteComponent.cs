
using System.Numerics;

namespace MysticEchoes.Core.Rendering;

public struct SpriteComponent
{
    public string Sprite { get; set; }
    public Vector2 LocalOffset { get; set; }
    public bool ReflectByY { get; set; }

    public SpriteComponent()
    {
        LocalOffset = Vector2.Zero;
        ReflectByY = false;
    }
}
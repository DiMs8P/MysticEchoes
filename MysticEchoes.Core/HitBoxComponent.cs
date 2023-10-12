using System.Drawing;
using MysticEchoes.Core.Base;

namespace MysticEchoes.Core;

public class HitBoxComponent : IComponent
{
    public Shape HitBox { get; set; }

    public bool CheckCollision(Rectangle other)
    {
        throw new NotImplementedException();

        return HitBox.IsIntersectWith(other);
    }
}

public class Shape
{
    public bool IsIntersectWith(Rectangle rectangle)
    {
        throw new NotImplementedException();
        return true;
    }
}
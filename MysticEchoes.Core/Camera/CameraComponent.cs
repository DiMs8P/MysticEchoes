using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MysticEchoes.Core.Camera;

public struct CameraComponent
{
    public Vector2 Position { get; set; }
    public Vector4 Speed { get; set; }

    public CameraComponent()
    {
        Speed = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
    }
}


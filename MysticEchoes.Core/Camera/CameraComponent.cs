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
    public Vector3 Front { get; set; }
    public Vector3 Up { get; set; }
    public Vector3 Right { get; set; }
    public Vector4 Speed { get; set; }

    public CameraComponent()
    {
        Front = new Vector3(0.0f, 0.0f, -1.0f);
        Up = new Vector3(0.0f, 1.0f, 0.0f);
        Right = new Vector3(0.0f, 1.0f, -1.0f);
        Speed = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
    }
}


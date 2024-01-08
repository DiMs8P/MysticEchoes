using System.Numerics;

namespace MysticEchoes.Core;

public class GameState
{
    public bool IsGameOver { get; set; }
    public Vector2 PlayerDeathPosition { get; set; }
    public Vector2 LastCameraPosition { get; set; }
}
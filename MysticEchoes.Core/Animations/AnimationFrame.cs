
namespace MysticEchoes.Core.Animations;

public class AnimationFrame
{
    public string Sprite { get; set; }
    public float CurrentFrameDuration { get; set; }
    public AnimNotify[]? AnimNotifies { get; set; }
}
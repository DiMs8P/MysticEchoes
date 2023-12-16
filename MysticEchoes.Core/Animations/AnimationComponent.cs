namespace MysticEchoes.Core.Animations;

public struct AnimationComponent
{
    public AnimationFrame[] Frames { get; set; }

    public float CurrentFrameElapsedTime { get; set; }
    public uint CurrentFrameIndex { get; set; }

    public AnimationComponent()
    {
        Frames = new AnimationFrame[]{};

        CurrentFrameElapsedTime = 0;
        CurrentFrameIndex = 0;
    }
}
namespace MysticEchoes.Core.Animations;

public struct AnimationComponent
{
    public AnimationState[] Frames { get; set; }

    public int CurrentFrameElapsedTime { get; set; }
    public uint CurrentFrameIndex { get; set; }

    public AnimationComponent()
    {
        Frames = new AnimationState[]{};

        CurrentFrameElapsedTime = 0;
        CurrentFrameIndex = 0;
    }
}
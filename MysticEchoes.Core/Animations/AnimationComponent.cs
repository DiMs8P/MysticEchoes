﻿namespace MysticEchoes.Core.Animations;

public struct AnimationComponent
{
    public string? AnimationId { get; set; }
    public float CurrentFrameElapsedTime { get; set; }
    public uint CurrentFrameIndex { get; set; }
    public bool IsActive { get; set; }
    public bool ReflectByY { get; set; }

    public AnimationComponent()
    {
        AnimationId = null;

        CurrentFrameElapsedTime = 0;
        CurrentFrameIndex = 0;

        IsActive = true;
        ReflectByY = false;
    }
}
namespace MysticEchoes.Core.Animations;

public struct CharacterAnimationComponent
{
    public Dictionary<CharacterState, AnimationState[]> Animations { get; set; }
    public CharacterState InitialState { get; set; }

    public CharacterAnimationComponent()
    {
        Animations = new Dictionary<CharacterState, AnimationState[]>();
        InitialState = CharacterState.None;
    }
}
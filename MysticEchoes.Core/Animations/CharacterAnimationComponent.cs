namespace MysticEchoes.Core.Animations;

public struct CharacterAnimationComponent
{
    public Dictionary<CharacterState, AnimationFrame[]> Animations { get; set; }
    public CharacterState InitialState { get; set; }

    public CharacterAnimationComponent()
    {
        Animations = new Dictionary<CharacterState, AnimationFrame[]>();
        InitialState = CharacterState.None;
    }
}
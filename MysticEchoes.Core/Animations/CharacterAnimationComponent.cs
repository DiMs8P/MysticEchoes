namespace MysticEchoes.Core.Animations;

public struct CharacterAnimationComponent
{
    public Dictionary<CharacterState, string> Animations { get; set; }
    public CharacterState CurrentState { get; set; }

    public CharacterAnimationComponent()
    {
        Animations = new Dictionary<CharacterState, string>();
        CurrentState = CharacterState.None;
    }
}
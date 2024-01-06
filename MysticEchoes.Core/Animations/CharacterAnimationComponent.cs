namespace MysticEchoes.Core.Animations;

public struct CharacterAnimationComponent
{
    public BaseStateMachine AnimationStateMachine { get; set; }
    public Dictionary<CharacterState, string> Animations { get; set; }
    public Dictionary<CharacterState, List<string>> MultipleAnimations { get; set; }
    public CharacterState CurrentState { get; set; }

    public CharacterAnimationComponent()
    {
        Animations = new Dictionary<CharacterState, string>();
        MultipleAnimations = new Dictionary<CharacterState, List<string>>();
        CurrentState = CharacterState.None;
    }
}
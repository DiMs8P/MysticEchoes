using Leopotam.EcsLite;
using MysticEchoes.Core.Events;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Animations;

public class AnimationStateMachineSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private GameplayEventListener _gameplayEventListener;
    
    private EcsWorld _world;
    private EcsFilter _animationFilter;
    
    private EcsPool<CharacterAnimationComponent> _stateMachines;
    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        
        _animationFilter = _world.Filter<AnimationComponent>().Inc<CharacterAnimationComponent>().End();
        _stateMachines = _world.GetPool<CharacterAnimationComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _animationFilter)
        {
            ref CharacterAnimationComponent stateMachineComponent = ref _stateMachines.Get(entityId);
            stateMachineComponent.AnimationStateMachine.Update();
        }
    }
}
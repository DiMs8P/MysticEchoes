using Leopotam.EcsLite;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Animations;

public class AnimationSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _systemExecutionContext;

    private EcsFilter _animationFilter;
    
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<SpriteComponent> _sprites;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _animationFilter = world.Filter<AnimationComponent>().Inc<SpriteComponent>().End();
        
        _animations = world.GetPool<AnimationComponent>();
        _sprites = world.GetPool<SpriteComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _animationFilter)
        {
            ref AnimationComponent animationComponent = ref _animations.Get(entityId);

            if (animationComponent.CurrentFrameElapsedTime > animationComponent.Frames[animationComponent.CurrentFrameIndex].CurrentFrameDuration)
            {
                if (++animationComponent.CurrentFrameIndex >= animationComponent.Frames.Length)
                {
                    animationComponent.CurrentFrameIndex = 0;
                }
                
                animationComponent.CurrentFrameElapsedTime = 0;
                
                ref SpriteComponent spriteComponent = ref _sprites.Get(entityId);
                spriteComponent.Sprite = animationComponent.Frames[animationComponent.CurrentFrameIndex].Sprite;
            }

            animationComponent.CurrentFrameElapsedTime += _systemExecutionContext.DeltaTime;
        }
    }
}
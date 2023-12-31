using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Animations;

public class AnimationSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _systemExecutionContext;
    [EcsInject] private AnimationManager _animationManager;

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

            if (!animationComponent.IsActive)
            {
                continue;
            }

            AnimationFrame[] animationFrames = _animationManager.GetAnimationFrames(animationComponent.AnimationId);
            if (animationComponent.CurrentFrameElapsedTime > animationFrames[animationComponent.CurrentFrameIndex].CurrentFrameDuration)
            {
                if (++animationComponent.CurrentFrameIndex >= animationFrames.Length)
                {
                    animationComponent.CurrentFrameIndex = 0;
                }
                
                animationComponent.CurrentFrameElapsedTime = 0;
                
                ref SpriteComponent spriteComponent = ref _sprites.Get(entityId);
                spriteComponent.Sprite = animationFrames[animationComponent.CurrentFrameIndex].Sprite;
            }

            animationComponent.CurrentFrameElapsedTime += _systemExecutionContext.DeltaTime;
        }
    }
}
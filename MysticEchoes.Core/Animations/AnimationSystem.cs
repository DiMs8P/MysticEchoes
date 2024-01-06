using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Animations;

public class AnimationSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _systemExecutionContext;
    [EcsInject] private AnimationManager _animationManager;

    private EcsWorld _world;
    private EcsFilter _animationFilter;
    
    private EcsPool<AnimationComponent> _animations;
    private EcsPool<SpriteComponent> _sprites;
    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _animationFilter = _world.Filter<AnimationComponent>().Inc<SpriteComponent>().End();
        
        _animations = _world.GetPool<AnimationComponent>();
        _sprites = _world.GetPool<SpriteComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _animationFilter)
        {
            ref AnimationComponent animationComponent = ref _animations.Get(entityId);

            if (!animationComponent.IsActive || animationComponent.AnimationId is null)
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

                AnimationFrame currentFrame = animationFrames[animationComponent.CurrentFrameIndex];
                
                ref SpriteComponent spriteComponent = ref _sprites.Get(entityId);
                spriteComponent.ReflectByY = animationComponent.ReflectByY;
                spriteComponent.Sprite = currentFrame.Sprite;
                
                if (currentFrame.AnimNotifies is not null)
                {
                    foreach (var animNotify in currentFrame.AnimNotifies)
                    {
                        animNotify.Notify(entityId, _world);
                    }
                }
            }

            animationComponent.CurrentFrameElapsedTime += _systemExecutionContext.DeltaTime;
        }
    }
}
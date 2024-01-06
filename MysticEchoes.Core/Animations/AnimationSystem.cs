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
            
            UpdateAnimationFrameAndSprite(ref animationComponent, entityId);
        }
    }
    
    private void UpdateAnimationFrameAndSprite(ref AnimationComponent animationComponent, int entityId)
    {
        AnimationFrame[] animationFrames = _animationManager.GetAnimationFrames(animationComponent.AnimationId!);
        ref SpriteComponent spriteComponent = ref _sprites.Get(entityId);
        
        AnimationFrame currentFrame = animationFrames[animationComponent.CurrentFrameIndex];
        bool shouldUpdateSprite = spriteComponent.Sprite != currentFrame.Sprite;
        bool shouldAdvanceFrame = animationComponent.CurrentFrameElapsedTime > currentFrame.CurrentFrameDuration;

        if (shouldUpdateSprite || shouldAdvanceFrame)
        {
            if (shouldAdvanceFrame)
            {
                AdvanceAnimationFrame(ref animationComponent, animationFrames.Length);
                currentFrame = animationFrames[animationComponent.CurrentFrameIndex];
            }

            UpdateSpriteComponent(ref spriteComponent, ref animationComponent, currentFrame);
            NotifyAnimationEvents(currentFrame, entityId);
        }
        else
        {
            animationComponent.CurrentFrameElapsedTime += _systemExecutionContext.DeltaTime;
        }
    }
    
    private void AdvanceAnimationFrame(ref AnimationComponent animationComponent, int totalFrames)
    {
        if (++animationComponent.CurrentFrameIndex >= totalFrames)
        {
            animationComponent.CurrentFrameIndex = 0;
        }
        animationComponent.CurrentFrameElapsedTime = 0;
    }
    
    private void UpdateSpriteComponent(ref SpriteComponent spriteComponent, ref AnimationComponent animationComponent, AnimationFrame currentFrame)
    {
        spriteComponent.ReflectByY = animationComponent.ReflectByY;
        spriteComponent.Sprite = currentFrame.Sprite;
    }

    private void NotifyAnimationEvents(AnimationFrame currentFrame, int entityId)
    {
        if (currentFrame.AnimNotifies is not null)
        {
            foreach (var animNotify in currentFrame.AnimNotifies)
            {
                animNotify.Notify(entityId, _world);
            }
        }
    }
}
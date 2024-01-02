using MysticEchoes.Core.Animations;

namespace MysticEchoes.Core.Loaders;

public class AnimationManager
{
    private Dictionary<string, AnimationFrame[]> _loadedAnimations;
    
    public AnimationManager(IDataLoader dataLoader)
    {
        _loadedAnimations = dataLoader.LoadAnimations();
    }
    
    public AnimationFrame[] GetAnimationFrames(string animationId)
    {
        if (_loadedAnimations.TryGetValue(animationId, out AnimationFrame[] animation))
        {
            return animation;
        }

        throw new ArgumentException($"Animation with '{animationId}' is not found.");
    }
}
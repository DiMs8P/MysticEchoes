using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;
using MysticEchoes.Core.AI.Tasks;

namespace MysticEchoes.Core.AI;

public class NecromancerBt : EcsBt
{
    public NecromancerBt(EcsWorld world, int ownerEntityId) : base(world, ownerEntityId){}
    protected override Node SetupTree()
    {
        Node root = new Attack(_world, _ownerEntityId);

        return root;
    }
}
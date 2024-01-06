using Leopotam.EcsLite;
using MysticEchoes.Core.AI.BehaviorTree;

namespace MysticEchoes.Core.AI;

public class NecromancerBt : EcsBt
{
    public NecromancerBt(EcsWorld world, int ownerEntityId) : base(world, ownerEntityId){}
    protected override Node SetupTree()
    {
        Node root = new Node();

        return root;
    }


}
using Leopotam.EcsLite;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;
using SharpGL.SceneGraph;
using System.Numerics;

namespace MysticEchoes.Core.Shooting;

public class PlayerShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;

    [EcsInject] private SystemExecutionContext _systemExecutionContext;

    private EcsFilter _playerFilter;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<WeaponComponent> _weapons;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        _transforms = world.GetPool<TransformComponent>();

        _playerFilter = world.Filter<PlayerMarker>().Inc<WeaponComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _weapons = world.GetPool<WeaponComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref TransformComponent transformComponent = ref _transforms.Get(playerId);
            transformComponent.Rotation = GetRotation(ref transformComponent);
            ref WeaponComponent playerWeapon = ref _weapons.Get(playerId);
            playerWeapon.State = _inputManager.IsShooting() ? WeaponState.WantsToFire : WeaponState.ReadyToFire;
        }
    }

    private Vector2 GetRotation(ref TransformComponent transformComponent)
    {
        //Matrix Mv = new Matrix(4, 4);
        //Matrix Mp = new Matrix(4, 4);
        Vector2 vector = _inputManager.GetMousePoint();
/*        Vector4 Vgood = new Vector4(vector, 0, 1);
        Mv = _systemExecutionContext.MatrixView;
        Mp = _systemExecutionContext.MatrixProjection;

        if (Mp is not null && Mv is not null)
        {
            Vgood = Mv.MultMatrix4OnVector4(new Vector4(transformComponent.Location, 0, 1));
            Vgood = Mp.MultMatrix4OnVector4(Vgood);
        }*/

        return vector - transformComponent.Location;
    }
}
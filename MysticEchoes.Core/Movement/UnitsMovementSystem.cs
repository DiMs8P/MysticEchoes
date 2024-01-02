using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Control;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Movement;

public class UnitsMovementSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _controlsFilter;
    private EcsPool<UnitControlComponent> _controls;
    private EcsPool<MovementComponent> _movements;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _controlsFilter = world.Filter<UnitControlComponent>().End();

        _movements = world.GetPool<MovementComponent>();
        _controls = world.GetPool<UnitControlComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _controlsFilter)
        {
            ref var control = ref _controls.Get(entity);
            ref var movement = ref _movements.Get(entity);

            movement.Velocity = control.MoveDirection;
        }
    }
}
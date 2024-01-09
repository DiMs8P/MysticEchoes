using Leopotam.EcsLite;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MysticEchoes.Core.Camera
{
    internal class CameraSystem : IEcsInitSystem, IEcsRunSystem
    {
        [EcsInject] private OpenGL _gl;
        private EcsFilter _playerFilter;

        private EcsPool<TransformComponent> _transforms;
        private EcsPool<CameraComponent> _cameras;
        private EcsPool<MovementComponent> _movements;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _playerFilter = world.Filter<PlayerMarker>().Inc<TransformComponent>().End();
            if (_playerFilter.GetEntitiesCount() != 1)
            {
                throw new Exception("Must be 1 player");
            }

            _transforms = world.GetPool<TransformComponent>();
            _cameras = world.GetPool<CameraComponent>();
            _movements = world.GetPool<MovementComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerId in _playerFilter)
            {
                float boxSize = 0.02f;
                ref TransformComponent transform = ref _transforms.Get(playerId);
                ref MovementComponent movement = ref _movements.Get(playerId);
                ref CameraComponent camera = ref _cameras.Get(playerId);

                _gl.LoadIdentity();
                Vector2 pos = GetDirection(transform.Location, ref camera, boxSize, ref movement);
                camera.Position = new Vector2(camera.Position.X + pos.X, camera.Position.Y + pos.Y);
                _gl.LookAt(camera.Position.X, camera.Position.Y, 3.0f, camera.Position.X, camera.Position.Y, 0.0f, 0.0f, 1.0f, 0.0f);
            }
        }

        public static Vector2 GetDirection(Vector2 trans, ref CameraComponent pos, float size, ref MovementComponent movement)
        {
            int i = 0;
            int j = 2;
            if (pos.Position.X > trans.X - 1 + size)
            {
                if (pos.Speed[0] >= -movement.Speed / 50)
                    pos.Speed = new Vector4(pos.Speed[0] - movement.Speed / 500, pos.Speed[1], pos.Speed[2], pos.Speed[3]);
                i = 0;
            }
            else
            {
                pos.Speed = new Vector4(0, pos.Speed[1], pos.Speed[2], pos.Speed[3]);
                i = 0;
            }
            if (pos.Position.X < trans.X - 1 - size)
            {
                if (pos.Speed[1] <= movement.Speed / 50)
                    pos.Speed = new Vector4(pos.Speed[0], pos.Speed[1] + movement.Speed / 500, pos.Speed[2], pos.Speed[3]);
                i = 1;
            }
            else
            {
                pos.Speed = new Vector4(pos.Speed[0], 0, pos.Speed[2], pos.Speed[3]);
                
            }
            if (pos.Position.Y > trans.Y - 1 + size)
            {
                if (pos.Speed[2] >= -movement.Speed / 50)
                    pos.Speed = new Vector4(pos.Speed[0], pos.Speed[1], pos.Speed[2] - movement.Speed / 500, pos.Speed[3]);
                j = 2;
            }
            else
            {
                pos.Speed = new Vector4(pos.Speed[0], pos.Speed[1], 0, pos.Speed[3]);
                j = 2;

            }
            if (pos.Position.Y < trans.Y - 1 - size)
            {
                if (pos.Speed[3] <= movement.Speed / 50)
                    pos.Speed = new Vector4(pos.Speed[0], pos.Speed[1], pos.Speed[2], pos.Speed[3] + movement.Speed / 500);
                j = 3;
            }
            else
            {
                pos.Speed = new Vector4(pos.Speed[0], pos.Speed[1], pos.Speed[2], 0);
               
            }

            return new Vector2(pos.Speed[i], pos.Speed[j]);
        }
    }
}

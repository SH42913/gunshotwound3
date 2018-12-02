using System;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2
{
    public class GunshotWound2Script : IDisposable
    {
        private EcsWorld _world;
        private EcsSystems _systems;
        
        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }

        public void Init()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            _systems
                .Add(new GswWorldInitSystem())
                .Add(new GswWorldSystem());
            _systems.Initialize();
            GameFiber.Yield();
        }

        public void Run()
        {
            while (IsRunning)
            {
                if (IsPaused)
                {
                    GameFiber.Yield();
                    continue;
                }
                
                _systems.Run();
                _world.RemoveOneFrameComponents();
                GameFiber.Yield();
            }
        }

        public void Dispose()
        {
            _world?.Dispose();
            _systems?.Dispose();
        }
    }
}
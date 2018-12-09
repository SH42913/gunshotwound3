using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GunshotWound2.Armor;
using GunshotWound2.GswWorld;
using GunshotWound2.HitDetecting;
using GunshotWound2.Utils;
using GunshotWound2.Weapons;
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

#if DEBUG
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private long _maxFrameTime;
#endif

        public void Init()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            _systems
                .Add(new GswWorldInitSystem())
                .Add(new WeaponInitSystem())
                .Add(new GswWorldSystem())
                .Add(new BaseHitDetectingSystem())
                .Add(new BodyHitDetectingSystem())
#if DEBUG
                .Add(new BodyHitHistoryShowSystem())
#endif
                .Add(new WeaponHitDetectingSystem())
                .Add(new WeaponHitValidatingSystem())
                .Add(new BaseHitCleanSystem())
                .Add(new ArmorHitProcessingSystem());
            _systems.Initialize();
            GameFiber.Yield();
        }

        public void Run()
        {
            while (IsRunning)
            {
#if DEBUG
                _stopwatch.Restart();
                if (Game.IsKeyDown(Keys.End))
                {
                    _maxFrameTime = 0;
                }
#endif
                if (IsPaused)
                {
                    GameFiber.Yield();
                    continue;
                }
                
                _systems.Run();
                _world.RemoveOneFrameComponents();
#if DEBUG
                _stopwatch.Stop();
                long elapsed = _stopwatch.ElapsedMilliseconds;
                if (elapsed > _maxFrameTime)
                {
                    _maxFrameTime = elapsed;
                }
                string worldTime = "Total/Max Time: " + elapsed + "/" + _maxFrameTime;
                worldTime.ShowInGsw(0.165f, 0.97f, 0.25f, Color.White);
#endif
                
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
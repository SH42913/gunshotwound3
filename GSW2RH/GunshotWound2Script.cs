using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GunshotWound2.Armor;
using GunshotWound2.BaseHitDetecting;
using GunshotWound2.Bodies;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.Weapons;
using GunshotWound2.Weapons.FireArms;
using GunshotWound2.Weapons.HitDetecting;
using GunshotWound2.WoundProcessing;
using GunshotWound2.WoundProcessing.Bleeding;
using GunshotWound2.WoundProcessing.Health;
using GunshotWound2.WoundProcessing.Pain;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2
{
    public class GunshotWound2Script : IDisposable
    {
        public const string WOUND_CONFIG_PATH = "\\Plugins\\GswConfigs\\GswWoundConfig.xml";
        public const string WORLD_CONFIG_PATH = "\\Plugins\\GswConfigs\\GswWorldConfig.xml";
        public const string WEAPON_CONFIG_PATH = "\\Plugins\\GswConfigs\\GswWeaponConfig.xml";
        public static int StatsContainerEntity { get; private set; }
        
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
            StatsContainerEntity = _world.CreateEntity();

            _systems
                .Add(new GswWorldInitSystem())
                .Add(new GswWorldSystem())
                .Add(new HealthInitSystem())
                .Add(new PedHealthInitSystem())
                .Add(new PainInitSystem())
                .Add(new PedPainInitSystem())
                .Add(new BleedingInitSystem())
                .Add(new PedBleedingInitSystem())
                .Add(new PedArmorInitSystem())
                .Add(new WeaponInitSystem())
                .Add(new WeaponArmorInitSystem())
                .Add(new FireArmsInitSystem())
                .Add(new BaseHitDetectingSystem())
                .Add(new BodyPartInitSystem())
                .Add(new BodyPartArmorInitSystem())
                .Add(new BodyHitDetectingSystem())
#if DEBUG
                .Add(new BodyHitHistoryShowSystem())
#endif
                .Add(new WeaponHitDetectingSystem())
                .Add(new WeaponHitValidatingSystem())
                .Add(new BaseHitCleanSystem())
                .Add(new HelmetHitProcessingSystem())
                .Add(new ArmorHitProcessingSystem())
                .Add(new FireArmsWoundSystem())
                .Add(new HealDetectSystem())
                .Add(new HealthSystem())
                .Add(new PainSystem())
                .Add(new PainStateSystem())
                .Add(new BleedingCleanSystem())
                .Add(new BleedingHealSystem())
                .Add(new BleedingCreateSystem())
                .Add(new BleedingSystem());
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
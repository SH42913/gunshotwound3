using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GunshotWound2.Armor.Systems;
using GunshotWound2.BaseHitDetecting.Systems;
using GunshotWound2.Bleeding.Systems;
using GunshotWound2.Bodies.Systems;
using GunshotWound2.Configs;
using GunshotWound2.Crits;
using GunshotWound2.GswWorld.Systems;
using GunshotWound2.Hashes;
using GunshotWound2.Health.Systems;
using GunshotWound2.Localization;
using GunshotWound2.Pain.Systems;
using GunshotWound2.Uids;
using GunshotWound2.Utils;
using GunshotWound2.Weapons.Systems;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2
{
    public class GunshotWound2Script : IDisposable
    {
        public const string WORLD_CONFIG_NAME = "GswWorldConfig.xml";
        public const string WEAPON_CONFIG_NAME = "GswWeaponConfig.xml";
        public const string WOUND_CONFIG_NAME = "GswWoundConfig.xml";
        
        public static readonly string[] CONFIG_NAMES =
        {
            WORLD_CONFIG_NAME,
            WEAPON_CONFIG_NAME,
            WOUND_CONFIG_NAME
        };

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
            StatsContainerEntity = _world.CreateEntityWith(out StatsContainerComponent _);

            _systems
                .Add(new ConfigInitSystem())
                .Add(new LocalizationInitSystem())
                .Add(new UidInitSystem())
                .Add(new HashesInitSystem())
                .Add(new GswWorldInitSystem())
                .Add(new GswWorldCleanSystem())
                .Add(new GswWorldSystem())
                .Add(new WoundInitSystem())
                .Add(new HealthInitSystem())
                .Add(new PainInitSystem())
                .Add(new BleedingInitSystem())
                .Add(new WeaponInitSystem())
                .Add(new ArmorInitSystem())
                .Add(new CritInitSystem())
                .Add(new BaseHitDetectingSystem())
                .Add(new BodyPartInitSystem())
                .Add(new BodyHitDetectingSystem())
#if DEBUG
                .Add(new BodyHitHistoryShowSystem())
#endif
                .Add(new WeaponHitDetectingSystem())
                .Add(new BaseHitCleanSystem())
                .Add(new HelmetHitProcessingSystem())
                .Add(new ArmorHitProcessingSystem())
                .Add(new CritSystem())
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
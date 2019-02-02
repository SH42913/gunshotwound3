using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GunshotWound2.Armor.Systems;
using GunshotWound2.BaseHitDetecting.Systems;
using GunshotWound2.Bleeding.Systems;
using GunshotWound2.BodyParts.Systems;
using GunshotWound2.Configs.Systems;
using GunshotWound2.Crits.Systems;
using GunshotWound2.Effects.FacialAnimation.Systems;
using GunshotWound2.Effects.InstantKill.Systems;
using GunshotWound2.Effects.NaturalMotion.Systems;
using GunshotWound2.Effects.Ragdoll.Systems;
using GunshotWound2.GswWorld.Systems;
using GunshotWound2.Hashes.Systems;
using GunshotWound2.Health.Systems;
using GunshotWound2.Localization.Systems;
using GunshotWound2.Pain.Systems;
using GunshotWound2.PainStates.Systems;
using GunshotWound2.Player.Systems;
using GunshotWound2.Uids.Systems;
using GunshotWound2.Weapons.Systems;
using GunshotWound2.Wounds.Systems;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2
{
    public class GunshotWound2Script : IDisposable
    {
        public const string BASE_CONFIG_NAME = "GswBaseConfig.xml";
        public const string BODY_PART_CONFIG_NAME = "GswBodyPartConfig.xml";
        public const string NM_CONFIG_NAME = "GswNaturalMotionConfig.xml";
        public const string PAIN_STATE_CONFIG_NAME = "GswPainStateConfig.xml";
        public const string PLAYER_CONFIG_NAME = "GswPlayerConfig.xml";
        public const string WEAPON_CONFIG_NAME = "GswWeaponConfig.xml";
        public const string WORLD_CONFIG_NAME = "GswWorldConfig.xml";
        public const string WOUND_CONFIG_NAME = "GswWoundConfig.xml";


        public static readonly string[] CONFIG_NAMES =
        {
            BASE_CONFIG_NAME,
            BODY_PART_CONFIG_NAME,
            NM_CONFIG_NAME,
            PAIN_STATE_CONFIG_NAME,
            PLAYER_CONFIG_NAME,
            WEAPON_CONFIG_NAME,
            WORLD_CONFIG_NAME,
            WOUND_CONFIG_NAME,
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
                .Add(new PlayerInitSystem())
                .Add(new WoundInitSystem())
                .Add(new HealthInitSystem())
                .Add(new PainInitSystem())
                .Add(new BleedingInitSystem())
                .Add(new WeaponInitSystem())
                .Add(new ArmorInitSystem())
                .Add(new CritInitSystem())
                .Add(new RagdollInitSystem())
                .Add(new PainStateInitSystem())
                .Add(new BodyPartInitSystem())
                .Add(new NaturalMotionInitSystem())
                .Add(new FacialAnimationInitSystem())
                .Add(new InstantKillInitSystem())
                .Add(new PainStateSystem())
                .Add(new BaseHitDetectingSystem())
                .Add(new BodyHitDetectingSystem())
#if DEBUG
                .Add(new BodyHitHistoryShowSystem())
#endif
                .Add(new WeaponHitDetectingSystem())
                .Add(new BaseHitCleanSystem())
                .Add(new HelmetHitProcessingSystem())
                .Add(new ArmorHitProcessingSystem())
                .Add(new WoundSystem())
                .Add(new HealDetectSystem())
                .Add(new CritSystem())
                .Add(new HealthSystem())
                .Add(new PainSystem())
                .Add(new BleedingCleanSystem())
                .Add(new BleedingHealSystem())
                .Add(new BleedingCreateSystem())
                .Add(new BleedingSystem())
                .Add(new RagdollSystem())
                .Add(new NaturalMotionSystem())
                .Add(new FacialAnimationSystem())
                .Add(new InstantKillSystem());
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
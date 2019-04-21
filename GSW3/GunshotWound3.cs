using System;
using GSW3.Armor.Systems;
using GSW3.BaseHitDetecting.Systems;
using GSW3.Bleeding.Systems;
using GSW3.BodyParts.Systems;
using GSW3.Configs.Systems;
using GSW3.ConsoleCommands.Systems;
using GSW3.Crits.Systems;
using GSW3.DebugSystems.DamagedBonesHistory.Systems;
using GSW3.DebugSystems.DebugText.Systems;
using GSW3.DebugSystems.FrameTime.Systems;
using GSW3.GswWorld.Systems;
using GSW3.Hashes.Systems;
using GSW3.Health.Systems;
using GSW3.Localization.Systems;
using GSW3.Notifications.Systems;
using GSW3.Pain.Systems;
using GSW3.PainStates.Systems;
using GSW3.Pause.Systems;
using GSW3.Player.Systems;
using GSW3.Uids.Systems;
using GSW3.Weapons.Systems;
using GSW3.WoundEffects.CameraShake.Systems;
using GSW3.WoundEffects.FacialAnimation.Systems;
using GSW3.WoundEffects.Flash.Systems;
using GSW3.WoundEffects.InstantKill.Systems;
using GSW3.WoundEffects.Movement.Systems;
using GSW3.WoundEffects.MovementClipset.Systems;
using GSW3.WoundEffects.NaturalMotion.Systems;
using GSW3.WoundEffects.PainSound.Systems;
using GSW3.WoundEffects.PedFlags.Systems;
using GSW3.WoundEffects.Ragdoll.Systems;
using GSW3.WoundEffects.ScreenEffect.Systems;
using GSW3.WoundEffects.SpecialAbilityLock.Systems;
using GSW3.WoundEffects.VehicleControl.Systems;
using GSW3.WoundEffects.WeaponDrop.Systems;
using GSW3.Wounds.Systems;
using Leopotam.Ecs;
using Rage;

namespace GSW3
{
    public class GunshotWound3 : IDisposable
    {
        internal static EcsEntity StatsContainerEntity { get; private set; }
        internal static EcsWorld EcsWorld { get; private set; }
        
        private EcsSystems _systems;
        private static readonly Random Random = new Random();

        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }

        public void Init()
        {
            EcsWorld?.Dispose();
            EcsWorld = new EcsWorld();
            _systems = new EcsSystems(EcsWorld);
            StatsContainerEntity = EcsWorld.CreateEntityWith(out StatsContainerComponent _);

            _systems
                .Add(new PauseDetectingSystem())
#if DEBUG
                .Add(new FrameTimeStartSystem())
#endif
                //InitSystems
                .Add(new ConfigInitSystem())
                .Add(new ConsoleCommandsInitSystem())
                .Add(new LocalizationInitSystem())
                .Add(new UidInitSystem())
                .Add(new HashesInitSystem())
                .Add(new NotificationsInitSystem())
                .Add(new DonateListSystem())
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
                .Add(new PainStateInitSystem())
                .Add(new CritInitSystem())
                .Add(new RagdollInitSystem())
                .Add(new BodyPartInitSystem())
                .Add(new NaturalMotionInitSystem())
                .Add(new FacialAnimationInitSystem())
                .Add(new InstantKillInitSystem())
                .Add(new WeaponDropInitSystem())
                .Add(new MovementInitSystem())
                .Add(new MovementClipsetInitSystem())
                .Add(new FlashInitSystem())
                .Add(new CameraShakeInitSystem())
                .Add(new ScreenEffectInitSystem())
                .Add(new PedFlagsInitSystem())
                .Add(new PlayPainInitSystem())
                .Add(new SpecialAbilityLockInitSystem())
                .Add(new VehicleControlInitSystem())
                //RunSystems
                .Add(new PainStateSystem())
                .Add(new BaseHitDetectingSystem())
                .Add(new BodyHitDetectingSystem())
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
                .Add(new InstantKillSystem())
                .Add(new WeaponDropSystem())
                .Add(new MovementSystem())
                .Add(new MovementClipsetSystem())
                .Add(new FlashSystem())
                .Add(new CameraShakeSystem())
                .Add(new ScreenEffectSystem())
                .Add(new PedFlagsSystem())
                .Add(new PlayPainSystem())
                .Add(new SpecialAbilityLockSystem())
                .Add(new VehicleControlSystem())
#if DEBUG
                .Add(new DamagedBoneHistorySystem())
                .Add(new FrameTimeStopSystem())
                .Add(new DebugTextSystem())
#endif
                .Add(new WoundNotificationSystem())
                .Add(new NotificationsSystem())
                .Inject(Random)
                .Inject(new GameService());
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
                EcsWorld.RemoveOneFrameComponents();
                GameFiber.Yield();
            }
        }

        public void Dispose()
        {
            EcsWorld?.Dispose();
            _systems?.Dispose();
        }
    }
}
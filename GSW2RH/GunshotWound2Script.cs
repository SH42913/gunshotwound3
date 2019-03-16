using System;
using GunshotWound2.Armor.Systems;
using GunshotWound2.BaseHitDetecting.Systems;
using GunshotWound2.Bleeding.Systems;
using GunshotWound2.BodyParts.Systems;
using GunshotWound2.Configs.Systems;
using GunshotWound2.Crits.Systems;
using GunshotWound2.GswWorld.Systems;
using GunshotWound2.Hashes.Systems;
using GunshotWound2.Health.Systems;
using GunshotWound2.Localization.Systems;
using GunshotWound2.Pain.Systems;
using GunshotWound2.PainStates.Systems;
using GunshotWound2.Pause.Systems;
using GunshotWound2.Player.Systems;
using GunshotWound2.Uids.Systems;
using GunshotWound2.Weapons.Systems;
using GunshotWound2.WoundEffects.CameraShake.Systems;
using GunshotWound2.Wounds.Systems;
using GunshotWound2.WoundEffects.FacialAnimation.Systems;
using GunshotWound2.WoundEffects.Flash.Systems;
using GunshotWound2.WoundEffects.InstantKill.Systems;
using GunshotWound2.WoundEffects.Movement.Systems;
using GunshotWound2.WoundEffects.MovementClipset.Systems;
using GunshotWound2.WoundEffects.NaturalMotion.Systems;
using GunshotWound2.WoundEffects.PedFlags.Systems;
using GunshotWound2.WoundEffects.Ragdoll.Systems;
using GunshotWound2.WoundEffects.ScreenEffect.Systems;
using GunshotWound2.WoundEffects.WeaponDrop.Systems;
using Leopotam.Ecs;
using Rage;
#if DEBUG
using GunshotWound2.DebugSystems.DamagedBonesHistory.Systems;
using GunshotWound2.DebugSystems.DebugText.Systems;
using GunshotWound2.DebugSystems.FrameTime.Systems;

#endif

namespace GunshotWound2
{
    public class GunshotWound2Script : IDisposable
    {
        public static int StatsContainerEntity { get; private set; }

        private EcsWorld _world;
        private EcsSystems _systems;

        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }

        public void Init()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            StatsContainerEntity = _world.CreateEntityWith(out StatsContainerComponent _);

            _systems
                .Add(new PauseDetectingSystem())
#if DEBUG
                .Add(new FrameTimeStartSystem())
#endif
                //InitSystems
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
                //CommonSystems
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
#if DEBUG
                .Add(new DamagedBoneHistorySystem())
                .Add(new FrameTimeStopSystem())
                .Add(new DebugTextSystem())
#endif
                ;
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
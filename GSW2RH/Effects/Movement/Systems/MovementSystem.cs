using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Effects.Movement.Systems
{
    [EcsInject]
    public class MovementSystem : BaseEffectSystem
    {
        public MovementSystem() : base(new GswLogger(typeof(MovementSystem)))
        {
        }

        protected override void PrepareRunActions()
        {
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
            var permanentRate = EcsWorld.GetComponent<PermanentMovementRateComponent>(pedEntity);
            if (permanentRate != null)
            {
                NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(ped, 1.0f);
                EcsWorld.RemoveComponent<PermanentMovementRateComponent>(pedEntity);
#if DEBUG
                Logger.MakeLog($"Permanent move rate for {pedEntity.GetEntityName(EcsWorld)} was reset");
#endif
            }

            var player = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity);
            if (player == null) return;

            NativeFunction.Natives.SET_PLAYER_SPRINT(Game.LocalPlayer, true);
            EcsWorld.RemoveComponent<PermanentDisabledSprintComponent>(pedEntity, true);
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            var permanentDisabled = EcsWorld.GetComponent<PermanentDisabledSprintComponent>(pedEntity);
            var player = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity);
            if (permanentDisabled == null && player != null)
            {
                var disable = EcsWorld.GetComponent<DisableSprintComponent>(woundEntity);
                if (disable != null)
                {
                    NativeFunction.Natives.SET_PLAYER_SPRINT(Game.LocalPlayer, false);
                    if (disable.Permanent)
                    {
                        EcsWorld.AddComponent<PermanentDisabledSprintComponent>(pedEntity);
                    }
#if DEBUG
                    Logger.MakeLog($"Sprint disabled for player, permanent = {disable.Permanent}");
#endif
                }

                var enable = EcsWorld.GetComponent<EnableSprintComponent>(woundEntity);
                if (enable != null)
                {
                    NativeFunction.Natives.SET_PLAYER_SPRINT(Game.LocalPlayer, true);
#if DEBUG
                    Logger.MakeLog("Sprint enabled for player");
#endif
                }
            }

            var permanentRate = EcsWorld.GetComponent<PermanentDisabledSprintComponent>(pedEntity);
            if (permanentRate == null)
            {
                var newRate = EcsWorld.GetComponent<NewMovementRateComponent>(woundEntity);
                if (newRate != null)
                {
                    NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(ped, newRate.Rate);
                    if (newRate.Permanent)
                    {
                        EcsWorld.AddComponent<PermanentDisabledSprintComponent>(pedEntity);
                    }
#if DEBUG
                    Logger.MakeLog($"Move rate for {pedEntity.GetEntityName(EcsWorld)} " +
                                   $"was changed to {newRate.Rate}, permanent = {newRate.Permanent}");
#endif
                }

                var restore = EcsWorld.GetComponent<RestoreMovementComponent>(woundEntity);
                if (restore != null)
                {
                    NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(ped, 1.0f);
#if DEBUG
                    Logger.MakeLog($"Move rate for {pedEntity.GetEntityName(EcsWorld)} was restored");
#endif
                }
            }
        }
    }
}
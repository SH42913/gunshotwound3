using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.WoundEffects.SpecialAbilityLock.Systems
{
    [EcsInject]
    public class SpecialAbilityLockSystem : BaseEffectSystem
    {
        public SpecialAbilityLockSystem() : base(new GswLogger(typeof(SpecialAbilityLockSystem)))
        {
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer) return;

            NativeFunction.Natives.SPECIAL_ABILITY_UNLOCK(ped.Model.Hash);
#if DEBUG
            Logger.MakeLog("Special Ability unlocked");
#endif
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer) return;

            var lockAbility = EcsWorld.GetComponent<LockSpecialAbilityComponent>(woundEntity);
            if (lockAbility != null)
            {
                if (NativeFunction.Natives.IS_SPECIAL_ABILITY_ACTIVE<bool>(Game.LocalPlayer))
                {
                    NativeFunction.Natives.SPECIAL_ABILITY_DEACTIVATE_FAST(Game.LocalPlayer);
#if DEBUG
                    Logger.MakeLog("Special Ability stopped");
#endif
                }

                NativeFunction.Natives.SPECIAL_ABILITY_LOCK(ped.Model.Hash);
#if DEBUG
                Logger.MakeLog("Special Ability locked");
#endif
            }

            var unlock = EcsWorld.GetComponent<UnlockSpecialAbilityComponent>(woundEntity);
            if (unlock != null)
            {
                NativeFunction.Natives.SPECIAL_ABILITY_UNLOCK(ped.Model.Hash);
#if DEBUG
                Logger.MakeLog("Special Ability unlocked");
#endif
            }
        }
    }
}
using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.WoundEffects.PedFlags.Systems
{
    [EcsInject]
    public class PedFlagsSystem : BaseEffectSystem
    {
        public PedFlagsSystem() : base(new GswLogger(typeof(PedFlagsSystem)))
        {
        }

        protected override void ResetEffect(Ped ped, EcsEntity pedEntity)
        {
            var changedFlags = EcsWorld.GetComponent<ChangedPedFlagsComponent>(pedEntity);
            if(changedFlags == null) return;

            foreach (int changedFlag in changedFlags.ChangedFlags)
            {
                NativeFunction.Natives.SET_PED_RESET_FLAG(ped, changedFlag, true);
            }
            changedFlags.ChangedFlags.Clear();
        }

        protected override void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            var changeFlag = EcsWorld.GetComponent<ChangePedFlagComponent>(woundEntity);
            if(changeFlag == null || !changeFlag.ForPlayer && isPlayer) return;

            var changedFlags = EcsWorld.EnsureComponent<ChangedPedFlagsComponent>(pedEntity, out _);
            NativeFunction.Natives.SET_PED_CONFIG_FLAG(ped, changeFlag.Id, changeFlag.Value);
            changedFlags.ChangedFlags.Add(changeFlag.Id);
#if DEBUG
            Logger.MakeLog($"Changed Flag {changeFlag.Id} to {changeFlag.Value} for {pedEntity.GetEntityName()}");
#endif
        }
    }
}
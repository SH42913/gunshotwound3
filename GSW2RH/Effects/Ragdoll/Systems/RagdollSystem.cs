using GunshotWound2.GswWorld;
using GunshotWound2.Health;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Effects.Ragdoll.Systems
{
    [EcsInject]
    public class RagdollSystem : BaseEffectSystem
    {
        private EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<WoundedComponent> _newPeds;
        private EcsFilter<GswPedComponent, CreatePermanentRagdollComponent> _needRagdollPeds;
        private EcsFilter<GswPedComponent, PermanentRagdollComponent, FullyHealedComponent> _healedPeds;

        public RagdollSystem() : base(new GswLogger(typeof(RagdollSystem)))
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            var permanent = EcsWorld.GetComponent<PermanentRagdollComponent>(pedEntity);
            var enable = EcsWorld.GetComponent<EnableRagdollComponent>(woundEntity);
            if (permanent == null && enable != null)
            {
                if (enable.Permanent)
                {
                    var create = EcsWorld.EnsureComponent<CreatePermanentRagdollComponent>(pedEntity, out _);
                    create.DisableOnlyOnHeal = enable.DisableOnlyOnHeal;
                    create.Type = enable.Type;
                }
                else
                {
                    int length = enable.LengthInMs;
                    NativeFunction
                        .Natives
                        .SET_PED_TO_RAGDOLL(ped, length, length, enable.Type, 0, 0, 0);
#if DEBUG
                    Logger.MakeLog($"{ped.Name(pedEntity)} got ragdoll for {length} ms");
#endif
                }
            }

            var disable = EcsWorld.GetComponent<DisablePermanentRagdollComponent>(woundEntity);
            if (permanent != null && !permanent.DisableOnlyOnHeal && disable != null)
            {
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 1, 1, 1, 0, 0, 0);
                EcsWorld.RemoveComponent<PermanentRagdollComponent>(pedEntity);
#if DEBUG
                Logger.MakeLog($"{ped.Name(pedEntity)} was restored from permanent ragdoll");
#endif
            }
        }

        protected override void PrepareRunActions()
        {
            foreach (int i in _healedPeds)
            {
                Ped ped = _healedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = _healedPeds.Entities[i];
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 1, 1, 1, 0, 0, 0);
                EcsWorld.RemoveComponent<PermanentRagdollComponent>(pedEntity);
#if DEBUG
                Logger.MakeLog($"{ped.Name(pedEntity)} was restored from permanent ragdoll");
#endif
            }

            foreach (int i in _newPeds)
            {
                Ped ped = _newPeds.Components1[i].ThisPed;
                if (!ped.Exists() && !ped.IsRagdoll) continue;

                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 1, 1, 1, 0, 0, 0);
            }

            foreach (int i in _needRagdollPeds)
            {
                Ped ped = _needRagdollPeds.Components1[i].ThisPed;
                int pedEntity = _needRagdollPeds.Entities[i];
                if (!ped.Exists() || EcsWorld.GetComponent<PermanentRagdollComponent>(pedEntity) != null)
                {
                    EcsWorld.RemoveComponent<CreatePermanentRagdollComponent>(pedEntity);
                    continue;
                }

                if (ped.IsRagdoll) continue;

                CreatePermanentRagdollComponent createRagdoll = _needRagdollPeds.Components2[i];
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, -1, -1, createRagdoll.Type, 0, 0, 0);

                EcsWorld
                    .EnsureComponent<PermanentRagdollComponent>(pedEntity, out _)
                    .DisableOnlyOnHeal = createRagdoll.DisableOnlyOnHeal;
                EcsWorld.RemoveComponent<CreatePermanentRagdollComponent>(pedEntity);
#if DEBUG
                Logger.MakeLog($"{ped.Name(pedEntity)} got permanent ragdoll");
#endif
            }
        }
    }
}
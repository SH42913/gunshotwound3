using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.WoundEffects.Ragdoll.Systems
{
    [EcsInject]
    public class RagdollSystem : BaseEffectSystem
    {
        private readonly EcsFilter<GswPedComponent, CreatePermanentRagdollComponent> _needRagdollPeds = null;
        private readonly EcsFilter<GswPedComponent, PermanentRagdollComponent> _permanentRagdollPeds = null;

        public RagdollSystem() : base(new GswLogger(typeof(RagdollSystem)))
        {
        }

        protected override void PreExecuteActions()
        {
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

                var permanentRagdoll = EcsWorld.EnsureComponent<PermanentRagdollComponent>(pedEntity, out _);
                permanentRagdoll.DisableOnlyOnHeal = createRagdoll.DisableOnlyOnHeal;
                permanentRagdoll.Type = createRagdoll.Type;
                
                EcsWorld.RemoveComponent<CreatePermanentRagdollComponent>(pedEntity);
#if DEBUG
                Logger.MakeLog($"{pedEntity.GetEntityName()} have got permanent ragdoll");
#endif
            }

            foreach (int i in _permanentRagdollPeds)
            {
                Ped ped = _permanentRagdollPeds.Components1[i].ThisPed;
                if (!ped.Exists() || ped.IsRagdoll) continue;

                PermanentRagdollComponent ragdoll = _permanentRagdollPeds.Components2[i];
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, -1, -1, ragdoll.Type, 0, 0, 0);
            }
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
            if (!ped.IsRagdoll) return;

            NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 1, 1, 1, 0, 0, 0);
#if DEBUG
            Logger.MakeLog($"{pedEntity.GetEntityName()} was restored from ragdoll");
#endif

            var permanent = EcsWorld.GetComponent<PermanentRagdollComponent>(pedEntity);
            if (permanent == null) return;

            EcsWorld.RemoveComponent<PermanentRagdollComponent>(pedEntity);
#if DEBUG
            Logger.MakeLog($"{pedEntity.GetEntityName()} was restored from permanent ragdoll");
#endif
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
                    Logger.MakeLog($"{pedEntity.GetEntityName()} got ragdoll for {length} ms");
#endif
                }
            }

            var disable = EcsWorld.GetComponent<DisablePermanentRagdollComponent>(woundEntity);
            if (permanent != null && !permanent.DisableOnlyOnHeal && disable != null)
            {
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 1, 1, 1, 0, 0, 0);
                EcsWorld.RemoveComponent<PermanentRagdollComponent>(pedEntity);
#if DEBUG
                Logger.MakeLog($"{pedEntity.GetEntityName()} was restored from permanent ragdoll");
#endif
            }
        }
    }
}
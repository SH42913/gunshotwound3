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
    public class RagdollSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, WoundedComponent> _woundedPeds;
        private EcsFilter<GswPedComponent, CreatePermanentRagdollComponent> _needRagdollPeds;
        private EcsFilter<GswPedComponent, PermanentRagdollComponent, FullyHealedComponent> _healedPeds;

        private readonly GswLogger _logger;

        public RagdollSystem()
        {
            _logger = new GswLogger(typeof(RagdollSystem));
        }

        public void Run()
        {
            foreach (int i in _healedPeds)
            {
                Ped ped = _healedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = _healedPeds.Entities[i];
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 1, 1, 1, 0, 0, 0);
                _ecsWorld.RemoveComponent<PermanentRagdollComponent>(pedEntity);
#if DEBUG
                _logger.MakeLog($"{ped.Name(pedEntity)} was restored from permanent ragdoll");
#endif
            }

            foreach (int i in _needRagdollPeds)
            {
                Ped ped = _needRagdollPeds.Components1[i].ThisPed;
                int pedEntity = _needRagdollPeds.Entities[i];
                if (!ped.Exists() || _ecsWorld.GetComponent<PermanentRagdollComponent>(pedEntity) != null)
                {
                    _ecsWorld.RemoveComponent<CreatePermanentRagdollComponent>(pedEntity);
                    continue;
                }

                if (ped.IsRagdoll) continue;

                CreatePermanentRagdollComponent createRagdoll = _needRagdollPeds.Components2[i];
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, -1, -1, createRagdoll.Type, 0, 0, 0);

                _ecsWorld
                    .EnsureComponent<PermanentRagdollComponent>(pedEntity, out _)
                    .DisableOnlyOnHeal = createRagdoll.DisableOnlyOnHeal;
                _ecsWorld.RemoveComponent<CreatePermanentRagdollComponent>(pedEntity);
#if DEBUG
                _logger.MakeLog($"{ped.Name(pedEntity)} got permanent ragdoll");
#endif
            }

            foreach (int pedIndex in _woundedPeds)
            {
                Ped ped = _woundedPeds.Components1[pedIndex].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = _woundedPeds.Entities[pedIndex];
                WoundedComponent wounded = _woundedPeds.Components2[pedIndex];

                foreach (int woundEntity in wounded.WoundEntities)
                {
                    var permanent = _ecsWorld.GetComponent<PermanentRagdollComponent>(pedEntity);
                    var enable = _ecsWorld.GetComponent<EnableRagdollComponent>(woundEntity);
                    if (permanent == null && enable != null)
                    {
                        if (enable.Permanent)
                        {
                            var create = _ecsWorld.EnsureComponent<CreatePermanentRagdollComponent>(pedEntity, out _);
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
                            _logger.MakeLog($"{ped.Name(pedEntity)} got ragdoll for {length} ms");
#endif
                        }
                    }

                    var disable = _ecsWorld.GetComponent<DisablePermanentRagdollComponent>(woundEntity);
                    if (permanent != null && !permanent.DisableOnlyOnHeal && disable != null)
                    {
                        NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 1, 1, 1, 0, 0, 0);
                        _ecsWorld.RemoveComponent<PermanentRagdollComponent>(pedEntity);
#if DEBUG
                        _logger.MakeLog($"{ped.Name(pedEntity)} was restored from permanent ragdoll");
#endif
                    }
                }
            }
        }
    }
}
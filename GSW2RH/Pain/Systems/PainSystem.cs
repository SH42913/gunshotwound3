using System;
using System.Drawing;
using GunshotWound2.BodyParts;
using GunshotWound2.GswWorld;
using GunshotWound2.Health;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Pain.Systems
{
    [EcsInject]
    public class PainSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<PainStatsComponent> _painStats;
        private EcsFilter<WoundedComponent, PainInfoComponent, DamagedBodyPartComponent> _woundedPeds;
        private EcsFilter<PainComponent, PainInfoComponent> _painToReduce;
        private EcsFilter<FullyHealedComponent, PainComponent> _healedEntities;
        private EcsFilter<PainIsGoneComponent> _entitiesToClean;
#if DEBUG
        private EcsFilter<GswPedComponent, PainComponent, PainInfoComponent> _pedsWithPain;
#endif

        private static readonly Random Random = new Random();
        private readonly GswLogger _logger;

        public PainSystem()
        {
            _logger = new GswLogger(typeof(PainSystem));
        }

        public void Run()
        {
            foreach (int i in _entitiesToClean)
            {
                int entity = _entitiesToClean.Entities[i];
                _ecsWorld.RemoveComponent<PainIsGoneComponent>(entity);
            }
            
            if (_painStats.EntitiesCount <= 0)
            {
                throw new Exception("PainSystem was not init!");
            }

            PainStatsComponent stats = _painStats.Components1[0];

            foreach (int i in _woundedPeds)
            {
                WoundedComponent wounded = _woundedPeds.Components1[i];
                PainInfoComponent painInfo = _woundedPeds.Components2[i];

                float maxPain = painInfo.UnbearablePain;
                int entity = _woundedPeds.Entities[i];

                float basePain = 0;
                foreach (int woundEntity in wounded.WoundEntities)
                {
                    var pain = _ecsWorld.GetComponent<BasePainComponent>(woundEntity);
                    if (pain == null) continue;

                    basePain += pain.BasePain;
#if DEBUG
                    _logger.MakeLog($"{woundEntity.GetEntityName(_ecsWorld)} increase pain for {pain.BasePain}");
#endif
                }

                var additionalPain = _ecsWorld.GetComponent<AdditionalPainComponent>(entity);
                if (additionalPain != null)
                {
                    basePain += additionalPain.AdditionalPain;
                }

                if (basePain <= 0) continue;

                int bodyPartEntity = _woundedPeds.Components3[i].DamagedBodyPartEntity;
                float bodyPartPainMult = _ecsWorld.GetComponent<PainMultComponent>(bodyPartEntity).Multiplier;
                float painWithMult = stats.PainMultiplier * bodyPartPainMult * basePain;

                float painDeviation = painWithMult * stats.PainDeviation;
                painDeviation = Random.NextFloat(-painDeviation, painDeviation);

                float finalPain = painWithMult + painDeviation;

                var painComponent = _ecsWorld.EnsureComponent<PainComponent>(entity, out bool isNew);
                if (isNew)
                {
                    painComponent.PainAmount = finalPain;
                }
                else
                {
                    painComponent.PainAmount += finalPain;
                }
#if DEBUG
                int pedEntity = _woundedPeds.Entities[i];
                float painPercent = painComponent.PainAmount / maxPain * 100f;
                _logger.MakeLog($"{pedEntity.GetEntityName(_ecsWorld)}: Base pain is {basePain:0.0}; " +
                                $"Final pain is {finalPain:0.0}; " +
                                $"Pain percent is {painPercent:0.0}");
#endif
            }

            foreach (int i in _painToReduce)
            {
                PainComponent painComponent = _painToReduce.Components1[i];
                float painRecoverySpeed = _painToReduce.Components2[i].PainRecoverySpeed;
                int entity = _painToReduce.Entities[i];

                painComponent.PainAmount -= painRecoverySpeed * GswExtensions.GetDeltaTime();
                if (painComponent.PainAmount > 0) continue;
                
                _ecsWorld.AddComponent<PainIsGoneComponent>(entity);
                _ecsWorld.RemoveComponent<PainComponent>(entity);
            }

            foreach (int i in _healedEntities)
            {
                int entity = _healedEntities.Entities[i];
                _ecsWorld.AddComponent<PainIsGoneComponent>(entity);
                _ecsWorld.RemoveComponent<PainComponent>(entity);
            }

#if DEBUG
            foreach (int i in _pedsWithPain)
            {
                Ped ped = _pedsWithPain.Components1[i].ThisPed;
                float pain = _pedsWithPain.Components2[i].PainAmount;
                float maxPain = _pedsWithPain.Components3[i].UnbearablePain;
                if (!ped.Exists() || pain <= 0) continue;

                Vector3 position = ped.AbovePosition + 0.2f * Vector3.WorldUp;
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(1.05f, 0.15f, 0.1f), Color.Orange);
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(pain / maxPain, 0.1f, 0.1f), Color.Red);
            }
#endif
        }
    }
}
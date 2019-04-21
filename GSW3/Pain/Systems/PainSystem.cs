using System;
using System.Drawing;
using GSW3.BodyParts;
using GSW3.GswWorld;
using GSW3.Health;
using GSW3.Utils;
using GSW3.Wounds;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Pain.Systems
{
    [EcsInject]
    public class PainSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly GameService _gameService = null;
        private readonly Random _random = null;
        
        private readonly EcsFilter<PainStatsComponent> _painStats = null;
        private readonly EcsFilter<WoundedComponent, PainInfoComponent, DamagedBodyPartComponent> _woundedPeds = null;
        private readonly EcsFilter<PainComponent, PainInfoComponent> _painToReduce = null;
        private readonly EcsFilter<FullyHealedComponent, PainComponent> _healedEntities = null;
        private readonly EcsFilter<PainIsGoneComponent> _entitiesToClean = null;
#if DEBUG
        private readonly EcsFilter<GswPedComponent, PainComponent, PainInfoComponent> _pedsWithPain = null;

        private readonly GswLogger _logger;

        public PainSystem()
        {
            _logger = new GswLogger(typeof(PainSystem));
        }
#endif

        public void Run()
        {
            foreach (int i in _entitiesToClean)
            {
                EcsEntity entity = _entitiesToClean.Entities[i];
                _ecsWorld.RemoveComponent<PainIsGoneComponent>(entity);
            }

            PainStatsComponent stats = _painStats.Components1[0];
            foreach (int i in _woundedPeds)
            {
                WoundedComponent wounded = _woundedPeds.Components1[i];
                EcsEntity entity = _woundedPeds.Entities[i];

                float basePain = 0;
                foreach (EcsEntity woundEntity in wounded.WoundEntities)
                {
                    var pain = _ecsWorld.GetComponent<BasePainComponent>(woundEntity);
                    if (pain == null) continue;

                    basePain += pain.BasePain;
#if DEBUG
                    _logger.MakeLog($"{woundEntity.GetEntityName()} increased pain for {pain.BasePain:0.00}");
#endif
                }

                var additionalPain = _ecsWorld.GetComponent<AdditionalPainComponent>(entity);
                if (additionalPain != null)
                {
                    basePain += additionalPain.AdditionalPain;
                }

                if (basePain <= 0) continue;
                EcsEntity bodyPartEntity = _woundedPeds.Components3[i].DamagedBodyPartEntity;
                float bodyPartPainMult = _ecsWorld.GetComponent<PainMultComponent>(bodyPartEntity).Multiplier;
                float painWithMult = stats.PainMultiplier * bodyPartPainMult * basePain;

                float painDeviation = painWithMult * stats.PainDeviation;
                painDeviation = _random.NextFloat(-painDeviation, painDeviation);
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
                float maxPain = _woundedPeds.Components2[i].UnbearablePain;
                EcsEntity pedEntity = _woundedPeds.Entities[i];
                float painPercent = painComponent.PainAmount / maxPain * 100f;
                _logger.MakeLog($"{pedEntity.GetEntityName()}: " +
                                $"Base pain is {basePain:0.00}; " +
                                $"Final pain is {finalPain:0.00}; " +
                                $"Pain percent is {painPercent:0.00}");
#endif
            }

            foreach (int i in _healedEntities)
            {
                EcsEntity entity = _healedEntities.Entities[i];
                _ecsWorld.AddComponent<PainIsGoneComponent>(entity);
                _ecsWorld.RemoveComponent<PainComponent>(entity);
            }

            if(_gameService.GameIsPaused) return;
            foreach (int i in _painToReduce)
            {
                PainComponent painComponent = _painToReduce.Components1[i];
                float painRecoverySpeed = _painToReduce.Components2[i].PainRecoverySpeed;
                EcsEntity entity = _painToReduce.Entities[i];

                painComponent.PainAmount -= painRecoverySpeed * GswExtensions.GetDeltaTime();
                if (painComponent.PainAmount > 0) continue;

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
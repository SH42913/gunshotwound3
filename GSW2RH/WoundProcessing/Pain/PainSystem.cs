using System;
using System.Drawing;
using GunshotWound2.Bodies;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.WoundProcessing.Pain
{
    [EcsInject]
    public class PainSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        
        private EcsFilter<PainWoundStatsComponent> _woundStats;
        private EcsFilter<ReceivedPainComponent, PainInfoComponent, DamagedBodyPartComponent> _painToIncrease;
        private EcsFilter<PainComponent, PainInfoComponent> _painToReduce;
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
            if(_woundStats.EntitiesCount <= 0) return;
            PainWoundStatsComponent woundStats = _woundStats.Components1[0];
            
            foreach (int i in _painToIncrease)
            {
                float basePain = _painToIncrease.Components1[i].Pain;
                float maxPain = _painToIncrease.Components2[i].UnbearablePain;
                float deadlyPain = woundStats.DeadlyPainMultiplier * maxPain;
                int entity = _painToIncrease.Entities[i];
                if(basePain <= 0) continue;

                int bodyPartEntity = _painToIncrease.Components3[i].DamagedBodyPartEntity;
                float bodyPartPainMult = _ecsWorld.GetComponent<PainMultComponent>(bodyPartEntity).Multiplier;
                float painWithMult = woundStats.PainMultiplier * bodyPartPainMult * basePain;
                
                float painDeviation = painWithMult * woundStats.PainDeviation;
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
                int pedEntity = _painToIncrease.Entities[i];
                _logger.MakeLog($"Entity ({pedEntity}): Base pain is {basePain:0.0}; " +
                                $"Final pain is {finalPain:0.0}; " +
                                $"New pain is {painComponent.PainAmount:0.0}/{maxPain:0.0}|{deadlyPain:0.0}");
#endif
            }

            foreach (int i in _painToReduce)
            {
                PainComponent painComponent = _painToReduce.Components1[i];
                float painRecoverySpeed = _painToReduce.Components2[i].PainRecoverySpeed;
                int entity = _painToReduce.Entities[i];

                painComponent.PainAmount -= painRecoverySpeed * GswExtensions.GetDeltaTime();
                if (painComponent.PainAmount <= 0)
                {
                    _ecsWorld.RemoveComponent<PainComponent>(entity);
                }
            }

#if DEBUG
            foreach (int i in _pedsWithPain)
            {
                Ped ped = _pedsWithPain.Components1[i].ThisPed;
                float pain = _pedsWithPain.Components2[i].PainAmount;
                float maxPain = _pedsWithPain.Components3[i].UnbearablePain;
                float deadlyPain = woundStats.DeadlyPainMultiplier * maxPain;
                if (pain <= 0) continue;

                Vector3 position = ped.AbovePosition + 0.2f * Vector3.WorldUp;
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(1.05f, 0.15f, 0.1f), Color.Orange);
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(maxPain / deadlyPain, 0.15f, 0.1f), Color.Yellow);
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(pain / deadlyPain, 0.1f, 0.1f), Color.Red);
            }
#endif
        }
    }
}
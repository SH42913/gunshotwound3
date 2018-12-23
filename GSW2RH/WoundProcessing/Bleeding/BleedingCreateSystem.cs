using System;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Bleeding
{
    [EcsInject]
    public class BleedingCreateSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<BleedingWoundStatsComponent> _woundStats;
        private EcsFilter<BleedingInfoComponent, CreateBleedingEvent> _toCreate;

        private readonly GswLogger _logger;
        private static readonly Random Random = new Random();

        public BleedingCreateSystem()
        {
            _logger = new GswLogger(typeof(BleedingCreateSystem));
        }

        public void Run()
        {
            if(_woundStats.EntitiesCount <= 0) return;
            BleedingWoundStatsComponent stats = _woundStats.Components1[0];
            
            foreach (int i in _toCreate)
            {
                BleedingInfoComponent info = _toCreate.Components1[i];
                CreateBleedingEvent e = _toCreate.Components2[i];
                int pedEntity = _toCreate.Entities[i];

                while (e.BleedingToCreate.Count > 0)
                {
                    float baseSeverity = e.BleedingToCreate.Dequeue();
                    if (baseSeverity <= 0) continue;
                    
                    float sevWithMult = stats.BleedingMultiplier * baseSeverity;
                    float sevDeviation = sevWithMult * stats.BleedingDeviation;
                    sevDeviation = Random.NextFloat(-sevDeviation, sevDeviation);
                    float finalSeverity = sevWithMult + sevDeviation;
                
                    int bleedingEntity = _ecsWorld.CreateEntityWith(out BleedingComponent bleeding);
                    bleeding.Severity = finalSeverity;
#if DEBUG
                    _logger.MakeLog($"Created bleeding for Entity ({pedEntity}). " +
                                    $"Base severity {baseSeverity:0.00}; final severity {finalSeverity:0.00}");
#endif
                    info.BleedingEntities.Add(bleedingEntity);
                }
                
#if DEBUG
                _ecsWorld.ProcessDelayedUpdates();
                string bleedingList = "";
                foreach (int bleedEntity in info.BleedingEntities)
                {
                    bleedingList += $"{_ecsWorld.GetComponent<BleedingComponent>(bleedEntity).Severity} ";
                }
                _logger.MakeLog($"BleedingList: {bleedingList}");
#endif
            }
        }
    }
}
using System;
using GunshotWound2.BodyParts;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;

namespace GunshotWound2.Bleeding.Systems
{
    [EcsInject]
    public class BleedingCreateSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<BleedingStatsComponent> _bleedingStats = null;
        private readonly EcsFilter<BleedingInfoComponent, WoundedComponent, DamagedBodyPartComponent> _wounded = null;

        private readonly GswLogger _logger;
        private static readonly Random Random = new Random();

        public BleedingCreateSystem()
        {
            _logger = new GswLogger(typeof(BleedingCreateSystem));
        }

        public void Run()
        {
            BleedingStatsComponent stats = _bleedingStats.Components1[0];
            foreach (int i in _wounded)
            {
                BleedingInfoComponent info = _wounded.Components1[i];
                WoundedComponent wounded = _wounded.Components2[i];
                int pedEntity = _wounded.Entities[i];

                foreach (int woundEntity in wounded.WoundEntities)
                {
                    var baseBleeding = _ecsWorld.GetComponent<BaseBleedingComponent>(woundEntity);
                    if (baseBleeding == null) continue;

                    float baseSeverity = baseBleeding.BaseBleeding;
                    if (baseSeverity <= 0) continue;

                    int bodyPartEntity = _wounded.Components3[i].DamagedBodyPartEntity;
                    float bodyPartMult = _ecsWorld.GetComponent<BleedingMultComponent>(bodyPartEntity).Multiplier;
                    float sevWithMult = stats.BleedingMultiplier * bodyPartMult * baseSeverity;

                    float sevDeviation = sevWithMult * stats.BleedingDeviation;
                    sevDeviation = Random.NextFloat(-sevDeviation, sevDeviation);

                    float finalSeverity = sevWithMult + sevDeviation;

                    int bleedingEntity = _ecsWorld.CreateEntityWith(out BleedingComponent bleeding);
                    bleeding.Severity = finalSeverity;
#if DEBUG
                    _logger.MakeLog(
                        $"Created bleeding {woundEntity.GetEntityName()} for {pedEntity.GetEntityName()}. " +
                        $"Base severity {baseSeverity:0.00}; final severity {finalSeverity:0.00}");
#endif
                    info.BleedingEntities.Add(bleedingEntity);
                }

#if DEBUG
                _ecsWorld.ProcessDelayedUpdates();
                string bleedingList = "";
                foreach (int bleedEntity in info.BleedingEntities)
                {
                    bleedingList += $"{_ecsWorld.GetComponent<BleedingComponent>(bleedEntity).Severity:0.00} ";
                }

                _logger.MakeLog($"BleedingList: {bleedingList}");
#endif
            }
        }
    }
}
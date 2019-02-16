using System;
using System.Drawing;
using GunshotWound2.BodyParts;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Health.Systems
{
    [EcsInject]
    public class HealthSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<HealthStatsComponent> _healthStats = null;
        private readonly EcsFilter<GswPedComponent, HealthComponent, FullyHealedComponent> _fullyHealedPeds = null;
        private readonly EcsFilter<
                GswPedComponent,
                HealthComponent,
                WoundedComponent,
                DamagedBodyPartComponent>
            .Exclude<FullyHealedComponent> _woundedPeds = null;
#if DEBUG
        private readonly EcsFilter<GswPedComponent, HealthComponent> _pedsWithHealth = null;
#endif

        private readonly GswLogger _logger;
        private static readonly Random Random = new Random();

        public HealthSystem()
        {
            _logger = new GswLogger(typeof(HealthSystem));
        }

        public void Run()
        {
            HealthStatsComponent stats = _healthStats.Components1[0];
            foreach (int i in _fullyHealedPeds)
            {
                Ped ped = _fullyHealedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                HealthComponent health = _fullyHealedPeds.Components2[i];
                health.Health = health.MaxHealth;
                ped.SetHealth(health.Health);
            }

            foreach (int i in _woundedPeds)
            {
                int pedEntity = _woundedPeds.Entities[i];
#if DEBUG
                _logger.MakeLog($"{pedEntity.GetEntityName()} was wounded");
#endif
                Ped ped = _woundedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                HealthComponent health = _woundedPeds.Components2[i];
                WoundedComponent wounded = _woundedPeds.Components3[i];

                float baseDamage = 0;
                foreach (int woundEntity in wounded.WoundEntities)
                {
                    var damage = _ecsWorld.GetComponent<BaseDamageComponent>(woundEntity);
                    if (damage == null) continue;

                    baseDamage += damage.BaseDamage;
#if DEBUG
                    _logger.MakeLog($"{woundEntity.GetEntityName()} increase damage for {damage.BaseDamage:0.00}");
#endif
                }

                if (baseDamage <= 0) continue;
                int bodyPartEntity = _woundedPeds.Components4[i].DamagedBodyPartEntity;
                float bodyPartDamageMult = _ecsWorld.GetComponent<DamageMultComponent>(bodyPartEntity).Multiplier;
                float damageWithMult = stats.DamageMultiplier * bodyPartDamageMult * baseDamage;

                float damageDeviation = damageWithMult * stats.DamageDeviation;
                damageDeviation = Random.NextFloat(-damageDeviation, damageDeviation);

                float finalDamage = damageWithMult + damageDeviation;
                health.Health -= finalDamage;
                ped.SetHealth(health.Health);
#if DEBUG
                _logger.MakeLog($"{pedEntity.GetEntityName()}:" +
                                $"Base damage is {baseDamage:0.00}; " +
                                $"Final damage is {finalDamage:0.00}; " +
                                $"New health is {health.Health:0.00}/{health.MaxHealth:0.00}");
#endif
            }

#if DEBUG
            foreach (int i in _pedsWithHealth)
            {
                Ped ped = _pedsWithHealth.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                HealthComponent health = _pedsWithHealth.Components2[i];
                if (health.Health <= 0) continue;

                Vector3 position = ped.AbovePosition + 0.1f * Vector3.WorldUp;
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(1.05f, 0.15f, 0.1f), Color.Gold);
                Debug.DrawWireBoxDebug(position, ped.Orientation,
                    new Vector3(health.Health / health.MaxHealth, 0.1f, 0.1f), Color.Green);
            }
#endif
        }
    }
}
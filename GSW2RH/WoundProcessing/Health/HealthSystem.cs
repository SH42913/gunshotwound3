using System.Drawing;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.WoundProcessing.Health
{
    [EcsInject]
    public class HealthSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<MainWoundStatsComponent> _woundStats;
        private EcsFilter<GswPedComponent, HealthComponent, FullyHealedComponent> _fullyHealed;
        private EcsFilter<GswPedComponent, HealthComponent, ReceivedDamageComponent> _damagedPeds;
#if DEBUG
        private EcsFilter<GswPedComponent, HealthComponent> _pedsWithHealth;
#endif

        private readonly GswLogger _logger;

        public HealthSystem()
        {
            _logger = new GswLogger(typeof(HealthSystem));
        }
        
        public void Run()
        {
            if(_woundStats.EntitiesCount <= 0) return;
            MainWoundStatsComponent woundStats = _woundStats.Components1[0];
            
            foreach (int i in _fullyHealed)
            {
                Ped ped = _damagedPeds.Components1[i].ThisPed;
                HealthComponent health = _fullyHealed.Components2[i];
                health.Health = health.MaxHealth;
                ped.Health = (int) health.Health;
            }
            
            foreach (int i in _damagedPeds)
            {
                Ped ped = _damagedPeds.Components1[i].ThisPed;
                HealthComponent health = _damagedPeds.Components2[i];
                float baseDamage = _damagedPeds.Components3[i].Damage;
                if(baseDamage <= 0) continue;

                float damageWithMult = woundStats.DamageMultiplier * baseDamage;
                float damageDeviation = damageWithMult * woundStats.DamageDeviation;
                float finalDamage = damageWithMult + damageDeviation;
                health.Health -= finalDamage;
                ped.Health = (int) health.Health;
#if DEBUG
                _logger.MakeLog("Base damage is " + baseDamage + 
                                "; Final damage is " + finalDamage +
                                "; New health is " + health.Health);
#endif
            }

#if DEBUG
            foreach (int i in _pedsWithHealth)
            {
                Ped ped = _pedsWithHealth.Components1[i].ThisPed;
                HealthComponent health = _pedsWithHealth.Components2[i];
                if (health.Health <= 0) continue;

                Vector3 position = ped.AbovePosition + 0.1f * Vector3.WorldUp;
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(1.05f, 0.15f, 0.1f), Color.Red);
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(health.Health / health.MaxHealth, 0.1f, 0.1f), Color.Yellow);
            }
#endif
        }
    }
}
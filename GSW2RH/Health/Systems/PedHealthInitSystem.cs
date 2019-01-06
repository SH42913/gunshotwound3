using System;
using System.Xml.Linq;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Health.Systems
{
    [EcsInject]
    public class PedHealthInitSystem : BaseStatsInitSystem<PedHealthStatsComponent>, IEcsRunSystem
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }

        private EcsFilter<PedHealthStatsComponent> _healthStats;
        private EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newHumans;
        private EcsFilter<GswPedComponent, NewPedMarkComponent, AnimalMarkComponent> _newAnimals;
        private static readonly Random Random = new Random();

        public PedHealthInitSystem()
        {
            ConfigPath = GunshotWound2Script.WORLD_CONFIG_PATH;
            Logger = new GswLogger(typeof(PedHealthInitSystem));
        }
        
        protected override void FillWithDefaultValues(PedHealthStatsComponent stats)
        {
            stats.PedHealth = new MinMax
            {
                Min = 50,
                Max = 100
            };
        }

        protected override void FillWithConfigValues(PedHealthStatsComponent stats, XElement xmlRoot)
        {
            XElement pedHealth = xmlRoot.GetElement("PedHealth");
            
            var health = pedHealth.GetMinMax();
            if (!health.IsDisabled())
            {
                stats.PedHealth = health;
            }
        }
        
        public void Run()
        {
            if(_healthStats.EntitiesCount <= 0) return;
            PedHealthStatsComponent stats = _healthStats.Components1[0];
            
            foreach (int i in _newHumans)
            {
                Ped ped = _newHumans.Components1[i].ThisPed;
                int humanEntity = _newHumans.Entities[i];

                var health = EcsWorld.AddComponent<HealthComponent>(humanEntity);
                health.Health = Random.NextMinMax(stats.PedHealth);
                health.MaxHealth = (float) Math.Floor(health.Health);
                
                ped.SetMaxHealth(health.MaxHealth);
                ped.SetHealth(health.Health);
            }
            
            foreach (int i in _newAnimals)
            {
                Ped ped = _newAnimals.Components1[i].ThisPed;
                int animalEntity = _newAnimals.Entities[i];

                var health = EcsWorld.AddComponent<HealthComponent>(animalEntity);
                health.Health = ped.GetHealth();
                health.MaxHealth = (float) Math.Floor(health.Health);
                ped.SetMaxHealth(health.MaxHealth);
                ped.SetHealth(health.Health);
            }
        }
    }
}
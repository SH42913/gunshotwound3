using System;
using System.Xml.Linq;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.WoundProcessing.Health;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.WoundProcessing.Bleeding
{
    [EcsInject]
    public class PedBleedingInitSystem : BaseStatsInitSystem<PedBleedingStatsComponent>, IEcsRunSystem
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }

        private EcsFilter<PedBleedingStatsComponent> _bleedingStats;
        private EcsFilter<PedHealthStatsComponent> _healthStats;
        private EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newHumans;
        private EcsFilter<GswPedComponent, NewPedMarkComponent, AnimalMarkComponent> _newAnimals;
        private static readonly Random Random = new Random();

        public PedBleedingInitSystem()
        {
            ConfigPath = GunshotWound2Script.WORLD_CONFIG_PATH;
            Logger = new GswLogger(typeof(PedBleedingInitSystem));
        }
        
        protected override void FillWithDefaultValues(PedBleedingStatsComponent stats)
        {
            stats.PedBleedingHealRate = new MinMax
            {
                Min = 0.5f,
                Max = 1
            };
        }

        protected override void FillWithConfigValues(PedBleedingStatsComponent stats, XElement xmlRoot)
        {
            XElement pedBleeding = xmlRoot.GetElement("PedBleedingHealRate");
            
            var bleeding = pedBleeding.GetMinMax();
            if (!bleeding.IsDisabled())
            {
                stats.PedBleedingHealRate = bleeding;
            }
        }
        
        public void Run()
        {
            if(_bleedingStats.EntitiesCount <= 0) return;
            PedBleedingStatsComponent stats = _bleedingStats.Components1[0];
            
            foreach (int i in _newHumans)
            {
                int humanEntity = _newHumans.Entities[i];
                var info = EcsWorld.AddComponent<BleedingInfoComponent>(humanEntity);
                info.BleedingHealRate = Random.NextMinMax(stats.PedBleedingHealRate);
            }
            
            if(_healthStats.EntitiesCount <= 0) return;
            PedHealthStatsComponent healthStats = _healthStats.Components1[0];
            
            foreach (int i in _newAnimals)
            {
                Ped ped = _newAnimals.Components1[i].ThisPed;
                int animalEntity = _newAnimals.Entities[i];

                float healthPercent = ped.GetHealth() / healthStats.PedHealth.Max;
                var info = EcsWorld.AddComponent<BleedingInfoComponent>(animalEntity);
                info.BleedingHealRate = healthPercent * stats.PedBleedingHealRate.Max;
            }
        }
    }
}
using System;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.GswWorld;
using GunshotWound2.Health;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Bleeding.Systems
{
    [EcsInject]
    public class BleedingInitSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<LoadedConfigComponent> _loadedConfigs;
        private EcsFilter<LoadedItemConfigComponent> _initParts;

        private EcsFilter<PedBleedingStatsComponent> _bleedingStats;
        private EcsFilter<PedHealthStatsComponent> _healthStats;
        private EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newHumans;
        private EcsFilter<GswPedComponent, NewPedMarkComponent, AnimalMarkComponent> _newAnimals;
        private static readonly Random Random = new Random();

        private readonly GswLogger _logger;

        public BleedingInitSystem()
        {
            _logger = new GswLogger(typeof(BleedingInitSystem));
        }

        public void PreInitialize()
        {
            _logger.MakeLog("BleedingSystem is loading!");

            var stats = _ecsWorld.AddComponent<BleedingStatsComponent>(GunshotWound2Script.StatsContainerEntity);
            stats.BleedingMultiplier = 1f;
            stats.BleedingMultiplier = 0.2f;

            var pedStats = _ecsWorld.AddComponent<PedBleedingStatsComponent>(GunshotWound2Script.StatsContainerEntity);
            pedStats.PedBleedingHealRate = new MinMax
            {
                Min = 0.5f,
                Max = 1
            };

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement multElement = xmlRoot.Element("OverallBleedingMultiplier");
                if (multElement != null)
                {
                    stats.BleedingMultiplier = multElement.GetFloat();
                }

                XElement devElement = xmlRoot.Element("OverallBleedingDeviation");
                if (devElement != null)
                {
                    stats.BleedingDeviation = devElement.GetFloat();
                }

                XElement pedElement = xmlRoot.Element("PedBleedingHealRate");
                if (pedElement == null) continue;

                var bleeding = pedElement.GetMinMax();
                if (!bleeding.IsDisabled())
                {
                    pedStats.PedBleedingHealRate = bleeding;
                }
            }

            _logger.MakeLog(stats.ToString());
            _logger.MakeLog(pedStats.ToString());
            _logger.MakeLog("BleedingSystem loaded!");
        }

        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement partRoot = _initParts.Components1[i].ElementRoot;
                int partEntity = _initParts.Entities[i];
                
                XElement multElement = partRoot.Element("BleedingMult");
                if (multElement != null)
                {
                    var mult = _ecsWorld.AddComponent<BleedingMultComponent>(partEntity);
                    mult.Multiplier = multElement.GetFloat();
                }

                XElement baseBleedingElement = partRoot.Element("BaseBleeding");
                if (baseBleedingElement != null)
                {
                    var bleeding = _ecsWorld.AddComponent<BaseBleedingComponent>(partEntity);
                    bleeding.BaseBleeding = baseBleedingElement.GetFloat();
                }
            }
        }

        public void Run()
        {
            if (_bleedingStats.EntitiesCount <= 0)
            {
                throw new Exception("BleedingSystem was not init!");
            }
            PedBleedingStatsComponent stats = _bleedingStats.Components1[0];
            
            foreach (int i in _newHumans)
            {
                int humanEntity = _newHumans.Entities[i];
                var info = _ecsWorld.AddComponent<BleedingInfoComponent>(humanEntity);
                info.BleedingHealRate = Random.NextMinMax(stats.PedBleedingHealRate);
            }

            if (_healthStats.EntitiesCount <= 0)
            {
                throw new Exception("HealthSystem was not init!");
            }
            PedHealthStatsComponent healthStats = _healthStats.Components1[0];
            
            foreach (int i in _newAnimals)
            {
                Ped ped = _newAnimals.Components1[i].ThisPed;
                int animalEntity = _newAnimals.Entities[i];

                float healthPercent = ped.GetHealth() / healthStats.PedHealth.Max;
                var info = _ecsWorld.AddComponent<BleedingInfoComponent>(animalEntity);
                info.BleedingHealRate = healthPercent * stats.PedBleedingHealRate.Max;
            }
        }

        public void PreDestroy()
        {
        }

        public void Destroy()
        {
        }
    }
}
using System;
using System.Xml.Linq;
using GSW3.Configs;
using GSW3.GswWorld;
using GSW3.Health;
using GSW3.Player;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Bleeding.Systems
{
    [EcsInject]
    public class BleedingInitSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly Random _random = null;

        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;
        private readonly EcsFilter<LoadedItemConfigComponent> _initParts = null;
        private readonly EcsFilter<PedBleedingStatsComponent> _bleedingStats = null;
        private readonly EcsFilter<PedHealthStatsComponent> _healthStats = null;
        private readonly EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newHumans = null;
        private readonly EcsFilter<GswPedComponent, NewPedMarkComponent, AnimalMarkComponent> _newAnimals = null;

        private readonly GswLogger _logger;

        public BleedingInitSystem()
        {
            _logger = new GswLogger(typeof(BleedingInitSystem));
        }

        public void PreInitialize()
        {
            EcsEntity mainEntity = GunshotWound3.StatsContainerEntity;
            var stats = _ecsWorld.AddComponent<BleedingStatsComponent>(mainEntity);
            stats.BleedingMultiplier = 1f;
            stats.BleedingDeviation = 0.2f;

            var pedStats = _ecsWorld.AddComponent<PedBleedingStatsComponent>(mainEntity);
            pedStats.AnimalMult = 1f;
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
                if (pedElement != null)
                {
                    var bleeding = pedElement.GetMinMax();
                    if (!bleeding.IsDisabled())
                    {
                        pedStats.PedBleedingHealRate = bleeding;
                    }

                    pedStats.AnimalMult = pedElement.GetFloat("AnimalMult");
                }

                XElement playerElement = xmlRoot.Element("PlayerBleedingHealRate");
                if (playerElement != null)
                {
                    pedStats.PlayerBleedingHealRate = playerElement.GetFloat();
                }
            }

#if DEBUG
            _logger.MakeLog(stats.ToString());
            _logger.MakeLog(pedStats.ToString());
#endif
            _logger.MakeLog("BleedingSystem loaded!");
        }

        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement partRoot = _initParts.Components1[i].ElementRoot;
                EcsEntity partEntity = _initParts.Entities[i];

                XElement multElement = partRoot.Element("BleedingMult");
                if (multElement != null)
                {
                    var mult = _ecsWorld.AddComponent<BleedingMultiplierComponent>(partEntity);
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
            PedBleedingStatsComponent stats = _bleedingStats.Components1[0];
            foreach (int i in _newHumans)
            {
                EcsEntity humanEntity = _newHumans.Entities[i];
                bool isPlayer = _ecsWorld.GetComponent<PlayerMarkComponent>(humanEntity) != null;

                var info = _ecsWorld.AddComponent<PedBleedingInfoComponent>(humanEntity);
                info.BleedingHealRate = isPlayer
                    ? stats.PlayerBleedingHealRate
                    : _random.NextMinMax(stats.PedBleedingHealRate);
            }

            PedHealthStatsComponent healthStats = _healthStats.Components1[0];
            foreach (int i in _newAnimals)
            {
                Ped ped = _newAnimals.Components1[i].ThisPed;
                EcsEntity animalEntity = _newAnimals.Entities[i];

                float healthPercent = ped.GetHealth() / healthStats.PedHealth.Max;
                var info = _ecsWorld.AddComponent<PedBleedingInfoComponent>(animalEntity);
                info.BleedingHealRate = healthPercent * stats.PedBleedingHealRate.Max * stats.AnimalMult;
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
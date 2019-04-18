using System;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.GswWorld;
using GunshotWound2.Health;
using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Pain.Systems
{
    [EcsInject]
    public class PainInitSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly Random _random = null;

        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;
        private readonly EcsFilter<LoadedItemConfigComponent> _initParts = null;
        private readonly EcsFilter<PedPainStatsComponent> _painStats = null;
        private readonly EcsFilter<PedHealthStatsComponent> _healthStats = null;
        private readonly EcsFilter<NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newHumans = null;
        private readonly EcsFilter<GswPedComponent, NewPedMarkComponent, AnimalMarkComponent> _newAnimals = null;

        private readonly GswLogger _logger;

        public PainInitSystem()
        {
            _logger = new GswLogger(typeof(PainInitSystem));
        }

        public void PreInitialize()
        {
            EcsEntity mainEntity = GunshotWound2Script.StatsContainerEntity;
            var stats = _ecsWorld.AddComponent<PainStatsComponent>(mainEntity);
            stats.PainMultiplier = 1f;
            stats.PainDeviation = 0.2f;

            var pedStats = _ecsWorld.AddComponent<PedPainStatsComponent>(mainEntity);
            pedStats.AnimalMult = 1f;
            pedStats.PlayerUnbearablePain = 100f;
            pedStats.PlayerPainRecoverySpeed = 1f;
            pedStats.PedUnbearablePain = new MinMax
            {
                Min = 50,
                Max = 100
            };
            pedStats.PedPainRecoverySpeed = new MinMax
            {
                Min = 0.5f,
                Max = 1f
            };

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement multElement = xmlRoot.Element("OverallPainMultiplier");
                if (multElement != null)
                {
                    stats.PainMultiplier = multElement.GetFloat();
                }

                XElement devElement = xmlRoot.Element("OverallPainDeviation");
                if (devElement != null)
                {
                    stats.PainDeviation = devElement.GetFloat();
                }

                XElement pedPainElement = xmlRoot.Element("PedUnbearablePain");
                if (pedPainElement != null)
                {
                    var pain = pedPainElement.GetMinMax();
                    if (!pain.IsDisabled())
                    {
                        pedStats.PedUnbearablePain = pain;
                    }

                    pedStats.AnimalMult = pedPainElement.GetFloat("AnimalMult");
                }

                XElement pedSpeedElement = xmlRoot.Element("PedPainRecoverySpeed");
                if (pedSpeedElement != null)
                {
                    var speed = pedSpeedElement.GetMinMax();
                    if (!speed.IsDisabled())
                    {
                        pedStats.PedPainRecoverySpeed = speed;
                    }
                }

                XElement playerPainElement = xmlRoot.Element("PlayerUnbearablePain");
                if (playerPainElement != null)
                {
                    pedStats.PlayerUnbearablePain = playerPainElement.GetFloat();
                }

                XElement playerSpeedElement = xmlRoot.Element("PlayerPainRecoverySpeed");
                if (playerSpeedElement != null)
                {
                    pedStats.PlayerPainRecoverySpeed = playerSpeedElement.GetFloat();
                }
            }

#if DEBUG
            _logger.MakeLog(stats.ToString());
            _logger.MakeLog(pedStats.ToString());
#endif
            _logger.MakeLog("PainSystem loaded!");
        }

        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement partRoot = _initParts.Components1[i].ElementRoot;
                EcsEntity partEntity = _initParts.Entities[i];

                XElement multElement = partRoot.Element("PainMult");
                if (multElement != null)
                {
                    var mult = _ecsWorld.AddComponent<PainMultComponent>(partEntity);
                    mult.Multiplier = multElement.GetFloat();
                }

                XElement basePainElement = partRoot.Element("BasePain");
                if (basePainElement != null)
                {
                    var pain = _ecsWorld.AddComponent<BasePainComponent>(partEntity);
                    pain.BasePain = basePainElement.GetFloat();
                }
            }
        }

        public void Run()
        {
            if (_painStats.IsEmpty())
            {
                throw new Exception("PainSystem was not init!");
            }

            PedPainStatsComponent stats = _painStats.Components1[0];
            foreach (int i in _newHumans)
            {
                EcsEntity humanEntity = _newHumans.Entities[i];
                bool isPlayer = _ecsWorld.GetComponent<PlayerMarkComponent>(humanEntity) != null;

                var painInfo = _ecsWorld.AddComponent<PainInfoComponent>(humanEntity);
                painInfo.UnbearablePain = isPlayer
                    ? stats.PlayerUnbearablePain
                    : _random.NextMinMax(stats.PedUnbearablePain);
                painInfo.PainRecoverySpeed = isPlayer
                    ? stats.PlayerPainRecoverySpeed
                    : _random.NextMinMax(stats.PedPainRecoverySpeed);
            }

            PedHealthStatsComponent healthStats = _healthStats.Components1[0];
            foreach (int i in _newAnimals)
            {
                Ped ped = _newAnimals.Components1[i].ThisPed;
                EcsEntity animalEntity = _newAnimals.Entities[i];

                float healthPercent = ped.GetHealth() / healthStats.PedHealth.Max;
                var painInfo = _ecsWorld.AddComponent<PainInfoComponent>(animalEntity);
                painInfo.UnbearablePain = healthPercent * stats.PedUnbearablePain.Max;
                painInfo.PainRecoverySpeed = healthPercent * stats.PedPainRecoverySpeed.Max;
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
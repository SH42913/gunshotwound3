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
        private EcsWorld _ecsWorld;

        private EcsFilter<LoadedConfigComponent> _loadedConfigs;
        private EcsFilter<LoadedItemConfigComponent> _initParts;

        private EcsFilter<PedPainStatsComponent> _painStats;
        private EcsFilter<PedHealthStatsComponent> _healthStats;
        private EcsFilter<NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newHumans;
        private EcsFilter<GswPedComponent, NewPedMarkComponent, AnimalMarkComponent> _newAnimals;
        private static readonly Random Random = new Random();

        private readonly GswLogger _logger;

        public PainInitSystem()
        {
            _logger = new GswLogger(typeof(PainInitSystem));
        }

        public void PreInitialize()
        {
            _logger.MakeLog("PainSystem is loading!");

            var stats = _ecsWorld.AddComponent<PainStatsComponent>(GunshotWound2Script.StatsContainerEntity);
            stats.PainMultiplier = 1f;
            stats.PainDeviation = 0.2f;
            stats.DeadlyPainMultiplier = 2.5f;

            var pedStats = _ecsWorld.AddComponent<PedPainStatsComponent>(GunshotWound2Script.StatsContainerEntity);
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

                XElement deadlyMultElement = xmlRoot.Element("DeadlyPainMultiplier");
                if (deadlyMultElement != null)
                {
                    stats.DeadlyPainMultiplier = deadlyMultElement.GetFloat();
                }

                XElement pedPainElement = xmlRoot.Element("PedUnbearablePain");
                if (pedPainElement != null)
                {
                    var pain = pedPainElement.GetMinMax();
                    if (!pain.IsDisabled())
                    {
                        pedStats.PedUnbearablePain = pain;
                    }
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

            _logger.MakeLog(stats.ToString());
            _logger.MakeLog(pedStats.ToString());
            _logger.MakeLog("PainSystem loaded!");
        }

        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement partRoot = _initParts.Components1[i].ElementRoot;
                int partEntity = _initParts.Entities[i];
                
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
            if (_painStats.EntitiesCount <= 0)
            {
                throw new Exception("PainSystem was not init!");
            }
            PedPainStatsComponent stats = _painStats.Components1[0];

            foreach (int i in _newHumans)
            {
                int humanEntity = _newHumans.Entities[i];
                
                bool isPlayer = _ecsWorld.GetComponent<PlayerMarkComponent>(humanEntity) != null;

                var painInfo = _ecsWorld.AddComponent<PainInfoComponent>(humanEntity);
                painInfo.UnbearablePain = isPlayer 
                    ? stats.PlayerUnbearablePain 
                    : Random.NextMinMax(stats.PedUnbearablePain);
                painInfo.PainRecoverySpeed = isPlayer 
                    ? stats.PlayerPainRecoverySpeed 
                    : Random.NextMinMax(stats.PedPainRecoverySpeed);
                painInfo.CurrentPainState = PainStates.NONE;
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
                var painInfo = _ecsWorld.AddComponent<PainInfoComponent>(animalEntity);
                painInfo.UnbearablePain = healthPercent * stats.PedUnbearablePain.Max;
                painInfo.PainRecoverySpeed = healthPercent * stats.PedPainRecoverySpeed.Max;
                painInfo.CurrentPainState = PainStates.NONE;
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
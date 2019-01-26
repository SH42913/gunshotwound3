using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.GswWorld.Systems
{
    [EcsInject]
    public class GswWorldInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedConfigComponent> _loadedConfigs;

        private readonly GswLogger _logger;

        private const string DETECTING_TARGETS = "DetectingTargets";
        private const string ACCURACY = "PedAccuracy";
        private const string SHOOT_RATE = "PedShootRate";
        private const string SCAN_ONLY_DAMAGED = "ScanOnlyDamaged";

        public GswWorldInitSystem()
        {
            _logger = new GswLogger(typeof(GswWorldInitSystem));
        }

        public void PreInitialize()
        {
            _logger.MakeLog("GswWorld is loading!");
            
            var world = _ecsWorld.AddComponent<GswWorldComponent>(GunshotWound2Script.StatsContainerEntity);
            FillWithDefaultValues(world);
            
            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement targetsElement = xmlRoot.Element(DETECTING_TARGETS);
                if (targetsElement != null)
                {
                    world.HumanDetectingEnabled = targetsElement.GetBool("Humans");
                    world.AnimalDetectingEnabled = targetsElement.GetBool("Animals");
                }

                XElement accuracyElement = xmlRoot.Element(ACCURACY);
                if (accuracyElement != null)
                {
                    world.HumanAccuracy = accuracyElement.GetMinMax();
                }

                XElement shootRateElement = xmlRoot.Element(SHOOT_RATE);
                if (shootRateElement != null)
                {
                    world.HumanShootRate = shootRateElement.GetMinMax();
                }

                XElement onlyDamagedElement = xmlRoot.Element(SCAN_ONLY_DAMAGED);
                if (onlyDamagedElement != null)
                {
                    world.ScanOnlyDamaged = onlyDamagedElement.GetBool();
                }
            }
            
            _logger.MakeLog(world.ToString());
            _logger.MakeLog("GswWorld loaded!");
        }

        private void FillWithDefaultValues(GswWorldComponent stats)
        {
            stats.HumanDetectingEnabled = true;
            stats.AnimalDetectingEnabled = true;
            
            stats.HumanAccuracy = new MinMax
            {
                Min = 10,
                Max = 30
            };
            stats.HumanShootRate = new MinMax
            {
                Min = 10,
                Max = 30
            };
            
            stats.ScanOnlyDamaged = false;

            stats.MaxDetectTimeInMs = 5;
        }

        public void PreDestroy()
        {
        }
    }
}
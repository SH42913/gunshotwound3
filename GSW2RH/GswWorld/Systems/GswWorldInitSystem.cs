using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.GswWorld.Systems
{
    [EcsInject]
    public class GswWorldInitSystem : IEcsPreInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;

        private readonly GswLogger _logger;

        public GswWorldInitSystem()
        {
            _logger = new GswLogger(typeof(GswWorldInitSystem));
        }

        public void PreInitialize()
        {
            var world = _ecsWorld.AddComponent<GswWorldComponent>(GunshotWound2Script.StatsContainerEntity);
            world.HumanDetectingEnabled = true;
            world.AnimalDetectingEnabled = true;
            world.HumanAccuracy = new MinMax
            {
                Min = 10,
                Max = 30
            };
            world.HumanShootRate = new MinMax
            {
                Min = 10,
                Max = 30
            };
            world.ScanOnlyDamaged = false;
            world.MaxDetectTimeInMs = 5;

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement targetsElement = xmlRoot.Element("DetectingTargets");
                if (targetsElement != null)
                {
                    world.HumanDetectingEnabled = targetsElement.GetBool("Humans");
                    world.AnimalDetectingEnabled = targetsElement.GetBool("Animals");
                }

                XElement accuracyElement = xmlRoot.Element("PedAccuracy");
                if (accuracyElement != null)
                {
                    world.HumanAccuracy = accuracyElement.GetMinMax();
                }

                XElement shootRateElement = xmlRoot.Element("PedShootRate");
                if (shootRateElement != null)
                {
                    world.HumanShootRate = shootRateElement.GetMinMax();
                }

                XElement onlyDamagedElement = xmlRoot.Element("ScanOnlyDamaged");
                if (onlyDamagedElement != null)
                {
                    world.ScanOnlyDamaged = onlyDamagedElement.GetBool();
                }
            }

#if DEBUG
            _logger.MakeLog(world.ToString());
#endif
            _logger.MakeLog("GswWorld loaded!");
        }

        public void PreDestroy()
        {
        }
    }
}
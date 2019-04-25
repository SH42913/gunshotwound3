using System.Xml.Linq;
using GSW3.Configs;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage.Native;

namespace GSW3.GswWorld.Systems
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
            var world = _ecsWorld.AddComponent<GswWorldComponent>(GunshotWound3.StatsContainerEntity);
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
            world.MaxPedCountPerFrame = 5;

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

                XElement pedsPerFrame = xmlRoot.Element("MaxPedsPerFrame");
                if (pedsPerFrame != null)
                {
                    world.MaxPedCountPerFrame = pedsPerFrame.GetInt();
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
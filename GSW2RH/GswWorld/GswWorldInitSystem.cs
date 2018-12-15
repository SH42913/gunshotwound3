using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld
{
    [EcsInject]
    public class GswWorldInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;

        private const string CONFIG_PATH = "\\Plugins\\GswConfigs\\GswWorldConfig.xml";
        private const string TARGETS_KEY = "DetectingTargets";
        private const string SCAN_ONLY_DAMAGED_ELEMENT = "ScanOnlyDamaged";

        private readonly GswLogger _logger;

        public GswWorldInitSystem()
        {
            _logger = new GswLogger(typeof(GswWorldInitSystem));
        }

        public void PreInitialize()
        {
            var gswWorld = _ecsWorld.CreateEntityWith<GswWorldComponent>();
            FillWithDefaultValues(gswWorld);
            try
            {
                FillWithConfigValues(gswWorld);
            }
            catch (Exception e)
            {
                _logger.MakeLog(e.Message);
                FillWithDefaultValues(gswWorld);
            }
        }

        private void FillWithDefaultValues(GswWorldComponent gswWorld)
        {
            gswWorld.PedDetectingEnabled = true;
            gswWorld.AnimalDetectingEnabled = true;
            
            gswWorld.PedHealth = new MinMax
            {
                Min = 50,
                Max = 100
            };
            gswWorld.PedAccuracy = new MinMax
            {
                Min = 10,
                Max = 30
            };
            gswWorld.PedShootRate = new MinMax
            {
                Min = 10,
                Max = 30
            };

            gswWorld.ScanOnlyDamaged = false;

            gswWorld.PedsToEntityDict = new Dictionary<Ped, int>();
            gswWorld.NeedToCheckPeds = new Queue<Ped>();

            gswWorld.MaxDetectTimeInMs = 5;
        }

        private void FillWithConfigValues(GswWorldComponent gswWorld)
        {
            string fullPath = Environment.CurrentDirectory + CONFIG_PATH;
            var file = new FileInfo(fullPath);
            if (!file.Exists)
            {
                throw new Exception("Can't find " + fullPath);
            }

            XElement xmlRoot = XDocument.Load(file.OpenRead()).Root;
            if (xmlRoot == null)
            {
                throw new Exception("Can't find root in " + CONFIG_PATH);
            }

            XElement worldElement = xmlRoot.GetElement(TARGETS_KEY);
            XElement scanOnlyDamageElement = xmlRoot.GetElement(SCAN_ONLY_DAMAGED_ELEMENT);

            XElement pedHealth = xmlRoot.GetElement("PedHealth");
            XElement pedAccuracy = xmlRoot.GetElement("PedAccuracy");
            XElement pedShootRate = xmlRoot.GetElement("PedShootRate");

            gswWorld.PedDetectingEnabled = worldElement.GetBool("Peds");
            gswWorld.AnimalDetectingEnabled = worldElement.GetBool("Animals");

            var health = pedHealth.GetMinMax();
            if (!health.IsDisabled())
            {
                gswWorld.PedHealth = health;
            }

            gswWorld.PedAccuracy = pedAccuracy.GetMinMax();
            gswWorld.PedShootRate = pedShootRate.GetMinMax();
            
            gswWorld.ScanOnlyDamaged = scanOnlyDamageElement.GetBool();

            _logger.MakeLog("GswWorld is inited!");
#if DEBUG
            _logger.MakeLog(gswWorld.ToString());
#endif
        }

        public void PreDestroy()
        {
        }
    }
}
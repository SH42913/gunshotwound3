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
        private const string WORLD_ENABLED_ELEMENT = "GswWorldEnabled";
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

            gswWorld.ScanOnlyDamaged = false;

            gswWorld.PedsToEntityDict = new Dictionary<Ped, int>();
            gswWorld.NeedToCheckPeds = new Queue<Ped>();

            gswWorld.MaxDetectTimeInMs = 7;
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

            XElement worldElement = xmlRoot.GetElement(WORLD_ENABLED_ELEMENT);
            XElement scanOnlyDamageElement = xmlRoot.GetElement(SCAN_ONLY_DAMAGED_ELEMENT);

            gswWorld.PedDetectingEnabled = worldElement.GetBool();
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
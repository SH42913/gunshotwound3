using System;
using System.IO;
using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing
{
    [EcsInject]
    public class WoundInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;
        
        private const string CONFIG_PATH = "\\Plugins\\GswConfigs\\GswWoundConfig.xml";
        
        private readonly GswLogger _logger;

        public WoundInitSystem()
        {
            _logger = new GswLogger(typeof(WoundInitSystem));
        }
        
        public void PreInitialize()
        {
            var woundStats = _ecsWorld.CreateEntityWith<MainWoundStatsComponent>();
            FillWithDefaultValues(woundStats);
            try
            {
                FillWithConfigValues(woundStats);
            }
            catch (Exception e)
            {
                _logger.MakeLog(e.Message);
                FillWithDefaultValues(woundStats);
            }
        }

        private void FillWithDefaultValues(MainWoundStatsComponent stats)
        {
            stats.DamageMultiplier = 1f;
            stats.DamageDeviation = 0.2f;
            stats.PainMultiplier = 1f;
            stats.PainDeviation = 0.2f;
            stats.DeadlyPainMultiplier = 2.5f;
        }

        private void FillWithConfigValues(MainWoundStatsComponent stats)
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

            XElement damMultElement = xmlRoot.GetElement("DamageMultiplier");
            XElement damDevElement = xmlRoot.GetElement("DamageDeviation");
            XElement painMultElement = xmlRoot.GetElement("PainMultiplier");
            XElement painDevElement = xmlRoot.GetElement("PainDeviation");
            XElement deadlyPainElement = xmlRoot.GetElement("DeadlyPainMultiplier");


            stats.DamageMultiplier = damMultElement.GetFloat();
            stats.DamageDeviation = damDevElement.GetFloat();
            stats.PainMultiplier = painMultElement.GetFloat();
            stats.PainDeviation = painDevElement.GetFloat();
            stats.DeadlyPainMultiplier = deadlyPainElement.GetFloat();
            
#if DEBUG
            _logger.MakeLog(stats.ToString());
#endif
        }

        public void PreDestroy()
        {
        }
    }
}
using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.GswWorld.Systems
{
    [EcsInject]
    public class GswWorldInitSystem : BaseStatsInitSystem<GswWorldComponent>
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }

        public GswWorldInitSystem()
        {
            ConfigPath = GunshotWound2Script.WORLD_CONFIG_PATH;
            Logger = new GswLogger(typeof(GswWorldInitSystem));
        }
        
        protected override void FillWithDefaultValues(GswWorldComponent stats)
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

        protected override void FillWithConfigValues(GswWorldComponent stats, XElement xmlRoot)
        {
            XElement worldElement = xmlRoot.GetElement("DetectingTargets");
            XElement scanOnlyDamageElement = xmlRoot.GetElement("ScanOnlyDamaged");

            XElement pedAccuracy = xmlRoot.GetElement("PedAccuracy");
            XElement pedShootRate = xmlRoot.GetElement("PedShootRate");

            stats.HumanDetectingEnabled = worldElement.GetBool("Humans");
            stats.AnimalDetectingEnabled = worldElement.GetBool("Animals");

            stats.HumanAccuracy = pedAccuracy.GetMinMax();
            stats.HumanShootRate = pedShootRate.GetMinMax();

            stats.ScanOnlyDamaged = scanOnlyDamageElement.GetBool();
        }
    }
}
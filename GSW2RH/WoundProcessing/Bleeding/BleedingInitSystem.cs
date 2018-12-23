using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Bleeding
{
    [EcsInject]
    public class BleedingInitSystem : BaseStatsInitSystem<BleedingWoundStatsComponent>
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }

        public BleedingInitSystem()
        {
            ConfigPath = GunshotWound2Script.WOUND_CONFIG_PATH;
            Logger = new GswLogger(typeof(BleedingInitSystem));
        }
        
        protected override void FillWithDefaultValues(BleedingWoundStatsComponent stats)
        {
            stats.BleedingMultiplier = 1f;
            stats.BleedingDeviation = 0.2f;
        }

        protected override void FillWithConfigValues(BleedingWoundStatsComponent stats, XElement xmlRoot)
        {
            XElement bleedMultElement = xmlRoot.GetElement("BleedingMultiplier");
            XElement bleedDevElement = xmlRoot.GetElement("BleedingDeviation");
            
            stats.BleedingMultiplier = bleedMultElement.GetFloat();
            stats.BleedingDeviation = bleedDevElement.GetFloat();
        }
    }
}
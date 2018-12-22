using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Health
{
    [EcsInject]
    public class HealthInitSystem : BaseStatsInitSystem<HealthWoundStatsComponent>
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }

        public HealthInitSystem()
        {
            ConfigPath = GunshotWound2Script.WOUND_CONFIG_PATH;
            Logger = new GswLogger(typeof(HealthInitSystem));
        }
        
        protected override void FillWithDefaultValues(HealthWoundStatsComponent stats)
        {
            stats.DamageMultiplier = 1f;
            stats.DamageDeviation = 0.2f;
        }

        protected override void FillWithConfigValues(HealthWoundStatsComponent stats, XElement xmlRoot)
        {
            XElement damMultElement = xmlRoot.GetElement("DamageMultiplier");
            XElement damDevElement = xmlRoot.GetElement("DamageDeviation");
            
            stats.DamageMultiplier = damMultElement.GetFloat();
            stats.DamageDeviation = damDevElement.GetFloat();
        }
    }
}
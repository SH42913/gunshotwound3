using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Pain
{
    [EcsInject]
    public class PainInitSystem : BaseStatsInitSystem<PainWoundStatsComponent>
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }

        public PainInitSystem()
        {
            ConfigPath = GunshotWound2Script.WOUND_CONFIG_PATH;
            Logger = new GswLogger(typeof(PainInitSystem));
        }
        
        protected override void FillWithDefaultValues(PainWoundStatsComponent stats)
        {
            stats.PainMultiplier = 1f;
            stats.PainDeviation = 0.2f;
            stats.DeadlyPainMultiplier = 2.5f;
        }

        protected override void FillWithConfigValues(PainWoundStatsComponent stats, XElement xmlRoot)
        {
            XElement painMultElement = xmlRoot.GetElement("PainMultiplier");
            XElement painDevElement = xmlRoot.GetElement("PainDeviation");
            XElement deadlyPainElement = xmlRoot.GetElement("DeadlyPainMultiplier");
            
            stats.PainMultiplier = painMultElement.GetFloat();
            stats.PainDeviation = painDevElement.GetFloat();
            stats.DeadlyPainMultiplier = deadlyPainElement.GetFloat();
        }
    }
}
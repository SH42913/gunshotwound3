using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Pain.Systems
{
    [EcsInject]
    public class PainInitSystem : BaseStatsInitSystem<PainWoundStatsComponent>, IEcsInitSystem
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }
        
        private EcsFilter<InitElementComponent, HashesComponent> _initParts;

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
        
        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement weaponRoot = _initParts.Components1[i].ElementRoot;
                XElement multElement = weaponRoot.GetElement("PainMult");
                int weaponEntity = _initParts.Entities[i];

                var mult = EcsWorld.AddComponent<PainMultComponent>(weaponEntity);
                mult.Multiplier = multElement.GetFloat();

#if DEBUG
                string name = _initParts.Components2[i].Name;
                Logger.MakeLog($"{name} got {mult}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
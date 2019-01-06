using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Bleeding.Systems
{
    [EcsInject]
    public class BleedingInitSystem : BaseStatsInitSystem<BleedingWoundStatsComponent>, IEcsInitSystem
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }
        
        private EcsFilter<InitElementComponent, HashesComponent> _initParts;

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
        
        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement weaponRoot = _initParts.Components1[i].ElementRoot;
                XElement multElement = weaponRoot.GetElement("BleedingMult");
                int weaponEntity = _initParts.Entities[i];

                var mult = EcsWorld.AddComponent<BleedingMultComponent>(weaponEntity);
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
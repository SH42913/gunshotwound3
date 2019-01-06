using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Health.Systems
{
    [EcsInject]
    public class HealthInitSystem : BaseStatsInitSystem<HealthWoundStatsComponent>, IEcsInitSystem
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }
        
        private EcsFilter<InitElementComponent, HashesComponent> _initParts;

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
        
        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement weaponRoot = _initParts.Components1[i].ElementRoot;
                XElement multElement = weaponRoot.GetElement("DamageMult");
                int weaponEntity = _initParts.Entities[i];

                var damageMult = EcsWorld.AddComponent<DamageMultComponent>(weaponEntity);
                damageMult.Multiplier = multElement.GetFloat();

#if DEBUG
                string name = _initParts.Components2[i].Name;
                Logger.MakeLog($"{name} got {damageMult}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
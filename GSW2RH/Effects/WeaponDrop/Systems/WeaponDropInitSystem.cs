using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.WeaponDrop.Systems
{
    [EcsInject]
    public class WeaponDropInitSystem : BaseEffectInitSystem, IEcsPreInitSystem
    {
        private EcsFilter<LoadedConfigComponent> _loadedConfigs;
        
        public WeaponDropInitSystem() : base(new GswLogger(typeof(WeaponDropInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
        {
            var dropElement = partRoot.Element("DropWeapon");
            if(dropElement == null) return;

            EcsWorld.AddComponent<WeaponDropComponent>(partEntity);
        }

        public void PreInitialize()
        {
            var stats = EcsWorld.AddComponent<WeaponDropStatsComponent>(GunshotWound2Script.StatsContainerEntity);
            stats.PlayerCanDropWeapon = true;

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;
                
                XElement playerCanDrop = xmlRoot.Element("PlayerCanDropWeapon");
                if (playerCanDrop != null)
                {
                    stats.PlayerCanDropWeapon = playerCanDrop.GetBool();
                }
            }
        }

        public void PreDestroy()
        {
        }
    }
}
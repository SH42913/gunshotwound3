using System.Xml.Linq;
using GSW3.Configs;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.WeaponDrop.Systems
{
    [EcsInject]
    public class WeaponDropInitSystem : BaseEffectInitSystem, IEcsPreInitSystem
    {
        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;

        public WeaponDropInitSystem() : base(new GswLogger(typeof(WeaponDropInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            var dropElement = partRoot.Element("DropWeapon");
            if (dropElement == null) return;

            var component = EcsWorld.AddComponent<WeaponDropComponent>(partEntity);
            component.TakeCoverDuration = dropElement.GetInt("TakeCoverDuration");
            component.FleeIfHasNoWeapons = dropElement.GetBool("FleeIfHasNoWeapons");
        }

        public void PreInitialize()
        {
            var stats = EcsWorld.AddComponent<WeaponDropStatsComponent>(GunshotWound3.StatsContainerEntity);
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
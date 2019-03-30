using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Weapons.Systems
{
    [EcsInject]
    public class WeaponInitSystem : IEcsPreInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;

        private readonly GswLogger _logger;

        public WeaponInitSystem()
        {
            _logger = new GswLogger(typeof(WeaponInitSystem));
        }

        public void PreInitialize()
        {
            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement listElement = xmlRoot.Element("WeaponList");
                if (listElement != null)
                {
                    foreach (XElement weaponRoot in listElement.Elements("Weapon"))
                    {
                        CreateWeapon(weaponRoot);
                    }
                }
            }

            _logger.MakeLog("Weapon list loaded!");
        }

        private void CreateWeapon(XElement weaponRoot)
        {
            _ecsWorld.CreateEntityWith(out WeaponComponent _, out LoadedItemConfigComponent initComponent);
            initComponent.ElementRoot = weaponRoot;
        }

        public void PreDestroy()
        {
        }
    }
}
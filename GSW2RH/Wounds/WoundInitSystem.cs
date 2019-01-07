using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Wounds
{
    [EcsInject]
    public class WoundInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedConfigComponent> _loadedConfigs;

        private readonly GswLogger _logger;
        private const string WOUND_LIST = "WoundList";

        public WoundInitSystem()
        {
            _logger = new GswLogger(typeof(WoundInitSystem));
        }
        
        public void PreInitialize()
        {
            _logger.MakeLog("Wound list is loading!");

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement listElement = xmlRoot.Element(WOUND_LIST);
                if (listElement == null) continue;
                
                foreach (XElement weaponRoot in listElement.Elements("Wound"))
                {
                    CreateWound(weaponRoot);
                }
            }

            _logger.MakeLog("Wound list loaded!");
        }

        private void CreateWound(XElement woundRoot)
        {
            _ecsWorld.CreateEntityWith(out WoundComponent _, out LoadedItemConfigComponent initComponent);
            initComponent.ElementRoot = woundRoot;
        }

        public void PreDestroy()
        {
        }
    }
}
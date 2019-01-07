using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Localization
{
    [EcsInject]
    public class LocalizationInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedItemConfigComponent> _loadedItems;
        
        public void Initialize()
        {
            foreach (int i in _loadedItems)
            {
                XElement itemRoot = _loadedItems.Components1[i].ElementRoot;

                XElement localElement = itemRoot.Element("LocalizationKey");
                if(localElement == null) continue;

                int itemEntity = _loadedItems.Entities[i];
                _ecsWorld
                    .AddComponent<LocalizationKeyComponent>(itemEntity)
                    .Key = localElement.GetAttributeValue("Key");
            }
        }

        public void Destroy()
        {
        }
    }
}
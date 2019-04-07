using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Localization.Systems
{
    [EcsInject]
    public class LocalizationInitSystem : IEcsInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<LoadedItemConfigComponent> _loadedItems = null;

        public void Initialize()
        {
            foreach (int i in _loadedItems)
            {
                XElement itemRoot = _loadedItems.Components1[i].ElementRoot;
                XElement localElement = itemRoot.Element("LocalizationKey");
                if (localElement == null) continue;

                EcsEntity itemEntity = _loadedItems.Entities[i];
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
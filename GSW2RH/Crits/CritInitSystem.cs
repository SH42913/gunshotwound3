using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Crits
{
    [EcsInject]
    public class CritInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedItemConfigComponent> _loadedItems;

        private readonly GswLogger _logger;

        public CritInitSystem()
        {
            _logger = new GswLogger(typeof(CritInitSystem));
        }

        public void Initialize()
        {
            foreach (int i in _loadedItems)
            {
                XElement itemRoot = _loadedItems.Components1[i].ElementRoot;
                int entity = _loadedItems.Entities[i];

                XElement chanceElement = itemRoot.Element("CritChance");
                if (chanceElement != null)
                {
                    var chance = _ecsWorld.AddComponent<CritChanceComponent>(entity);
                    chance.CritChance = chanceElement.GetFloat();
                }
            }
        }

        public void Destroy()
        {
        }
    }
}
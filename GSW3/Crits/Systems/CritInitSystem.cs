using System.Xml.Linq;
using GSW3.Configs;
using GSW3.GswWorld;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.Crits.Systems
{
    [EcsInject]
    public class CritInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<LoadedItemConfigComponent> _loadedItems = null;
        private readonly EcsFilter<NewPedMarkComponent> _newPeds = null;

#if DEBUG
        private readonly GswLogger _logger;

        public CritInitSystem()
        {
            _logger = new GswLogger(typeof(CritInitSystem));
        }
#endif

        public void Initialize()
        {
            foreach (int i in _loadedItems)
            {
                XElement itemRoot = _loadedItems.Components1[i].ElementRoot;
                EcsEntity entity = _loadedItems.Entities[i];

                XElement chanceElement = itemRoot.Element("CritChance");
                if (chanceElement != null)
                {
                    var chance = _ecsWorld.AddComponent<CritChanceComponent>(entity);
                    chance.CritChance = chanceElement.GetFloat();
                }
            }
        }

        public void Run()
        {
            foreach (int i in _newPeds)
            {
                EcsEntity pedEntity = _newPeds.Entities[i];
                _ecsWorld.AddComponent<CritListComponent>(pedEntity);
            }
        }

        public void Destroy()
        {
        }
    }
}
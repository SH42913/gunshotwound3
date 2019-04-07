using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Crits.Systems
{
    [EcsInject]
    public class CritInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<LoadedItemConfigComponent> _loadedItems = null;
        private readonly EcsFilter<NewPedMarkComponent> _newPeds = null;

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
                EcsEntity entity = _loadedItems.Entities[i];

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

        public void Run()
        {
            foreach (int i in _newPeds)
            {
                EcsEntity pedEntity = _newPeds.Entities[i];
                _ecsWorld.AddComponent<CritListComponent>(pedEntity);
            }
        }
    }
}
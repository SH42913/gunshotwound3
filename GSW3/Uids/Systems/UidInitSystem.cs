using System.Xml.Linq;
using GSW3.Configs;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.Uids.Systems
{
    [EcsInject]
    public class UidInitSystem : IEcsInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<LoadedItemConfigComponent> _items = null;

        private readonly GswLogger _logger;

        public UidInitSystem()
        {
            _logger = new GswLogger(typeof(UidInitSystem));
        }

        public void Initialize()
        {
            var dict = _ecsWorld.AddComponent<UidToEntityDictComponent>(GunshotWound3.StatsContainerEntity);
            foreach (int i in _items)
            {
                XElement itemRoot = _items.Components1[i].ElementRoot;
                XElement uidElement = itemRoot.Element("Uid");
                if (uidElement == null) continue;

                EcsEntity entity = _items.Entities[i];
                string uid = uidElement.GetAttributeValue("Value");
                if (dict.UidToEntityDict.ContainsKey(uid))
                {
                    _logger.MakeLog($"!!!WARNING!!! Uid {uid} already exists! {itemRoot.Name} skipped!");
                    return;
                }

                _ecsWorld.AddComponent<UidComponent>(entity).Uid = uid;
                dict.UidToEntityDict.Add(uid, entity);

#if DEBUG
                _logger.MakeLog($"{entity.GetEntityName()} added to UidDict as {uid}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
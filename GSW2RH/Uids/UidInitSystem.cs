using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Uids
{
    [EcsInject]
    public class UidInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<LoadedItemConfigComponent> _items;

        private readonly GswLogger _logger;

        public UidInitSystem()
        {
            _logger = new GswLogger(typeof(UidInitSystem));
        }

        public void Initialize()
        {
            var dict = _ecsWorld.AddComponent<UidToEntityDictComponent>(GunshotWound2Script.StatsContainerEntity);
            
            foreach (int i in _items)
            {
                XElement itemRoot = _items.Components1[i].ElementRoot;
                XElement uidElement = itemRoot.Element("Uid");

                if (uidElement == null) continue;

                int entity = _items.Entities[i];
                long uid = uidElement.GetLong();

                _ecsWorld.AddComponent<UidComponent>(entity).Uid = uid;
                dict.UidToEntityDict.Add(uid, entity);

#if DEBUG
                _logger.MakeLog($"Entity {entity} was marked with Uid {uid}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
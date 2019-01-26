using System;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Uids;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Wounds.Systems
{
    [EcsInject]
    public class WoundInitSystem : IEcsPreInitSystem, IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedConfigComponent> _loadedConfigs;
        private EcsFilter<LoadedItemConfigComponent> _loadedItems;
        private EcsFilter<UidToEntityDictComponent> _uidDict;

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

                foreach (XElement woundRoot in listElement.Elements("Wound"))
                {
                    CreateWound(woundRoot);
                }
            }

            _logger.MakeLog("Wound list loaded!");
        }

        private void CreateWound(XElement woundRoot)
        {
            _ecsWorld.CreateEntityWith(out WoundComponent _, out LoadedItemConfigComponent initComponent);
            initComponent.ElementRoot = woundRoot;
        }

        public void Initialize()
        {
            if (_uidDict.EntitiesCount <= 0)
            {
                throw new Exception("UidSystem was not init!");
            }

            var uidDict = _uidDict.Components1[0].UidToEntityDict;

            foreach (int i in _loadedItems)
            {
                XElement itemRoot = _loadedItems.Components1[i].ElementRoot;
                XElement woundListElement = null;
                string elementKey = null;

                foreach (XElement child in itemRoot.Elements())
                {
                    string name = child.Name.LocalName;
                    int trimIndex = name.IndexOf("WoundList", StringComparison.Ordinal);
                    if (trimIndex < 0) continue;

                    elementKey = name.Substring(0, trimIndex);
                    woundListElement = child;
                }

                if (string.IsNullOrEmpty(elementKey) || !woundListElement.HasElements) continue;
                string woundElementName = elementKey + "Wound";

                int itemEntity = _loadedItems.Entities[i];
                var randomizer = _ecsWorld.AddComponent<WoundRandomizerComponent>(itemEntity);
                foreach (XElement woundElement in woundListElement.Elements(woundElementName))
                {
                    long woundUid = woundElement.GetLong("Uid");
                    if (!uidDict.ContainsKey(woundUid))
                    {
                        throw new Exception($"Entity with Uid {woundUid} not found!");
                    }

                    int woundEntity = uidDict[woundUid];
                    var wound = _ecsWorld.GetComponent<WoundComponent>(woundEntity);
                    if (wound == null)
                    {
                        throw new Exception($"Entity with Uid {woundUid} is not wound!");
                    }

                    int woundWeight = woundElement.GetInt("Weight");
                    randomizer.WoundRandomizer.Add(woundEntity, woundWeight);
                }
            }
        }

        public void PreDestroy()
        {
        }

        public void Destroy()
        {
        }
    }
}
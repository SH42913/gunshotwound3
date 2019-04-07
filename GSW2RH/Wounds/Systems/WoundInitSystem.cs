using System;
using System.Collections.Generic;
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
        private readonly EcsWorld _ecsWorld = null;
        
        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;
        private readonly EcsFilter<LoadedItemConfigComponent> _loadedItems = null;
        private readonly EcsFilter<UidToEntityDictComponent> _uidDict = null;

        private readonly GswLogger _logger;

        public WoundInitSystem()
        {
            _logger = new GswLogger(typeof(WoundInitSystem));
        }

        public void PreInitialize()
        {
            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;
                XElement listElement = xmlRoot.Element("WoundList");
                if (listElement == null) continue;

                foreach (XElement woundRoot in listElement.Elements("Wound"))
                {
                    CreateWound(woundRoot);
                }
            }

            _logger.MakeLog("WoundList loaded!");
        }

        private void CreateWound(XElement woundRoot)
        {
            _ecsWorld.CreateEntityWith(out WoundComponent _, out LoadedItemConfigComponent initComponent);
            initComponent.ElementRoot = woundRoot;
        }

        public void Initialize()
        {
            if (_uidDict.IsEmpty())
            {
                throw new Exception("UidSystem was not init!");
            }

            Dictionary<string, EcsEntity> uidDict = _uidDict.Components1[0].UidToEntityDict;
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

                EcsEntity itemEntity = _loadedItems.Entities[i];
                var randomizer = _ecsWorld.AddComponent<WoundRandomizerComponent>(itemEntity);
                foreach (XElement woundElement in woundListElement.Elements(woundElementName))
                {
                    string woundUid = woundElement.GetAttributeValue("Uid");
                    if (!uidDict.ContainsKey(woundUid))
                    {
                        throw new Exception($"Entity with Uid {woundUid} not found!");
                    }

                    EcsEntity woundEntity = uidDict[woundUid];
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
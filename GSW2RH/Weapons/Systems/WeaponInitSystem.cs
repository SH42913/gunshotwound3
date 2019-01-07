using System;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Hashes;
using GunshotWound2.Uids;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;

namespace GunshotWound2.Weapons.Systems
{
    [EcsInject]
    public class WeaponInitSystem : IEcsPreInitSystem, IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedConfigComponent> _loadedConfigs;
        private EcsFilter<LoadedItemConfigComponent, WeaponComponent> _loadedWeapons;
        private EcsFilter<UidToEntityDictComponent> _uidDict;

        private readonly GswLogger _logger;
        private const string WEAPON_LIST = "WeaponList";

        public WeaponInitSystem()
        {
            _logger = new GswLogger(typeof(WeaponInitSystem));
        }

        public void PreInitialize()
        {
            _logger.MakeLog("Weapon list is loading!");

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement listElement = xmlRoot.Element(WEAPON_LIST);
                if (listElement != null)
                {
                    foreach (XElement weaponRoot in listElement.Elements("Weapon"))
                    {
                        CreateWeapon(weaponRoot);
                    }
                }
            }

            _logger.MakeLog("Weapon list loaded!");
        }

        private void CreateWeapon(XElement weaponRoot)
        {
            _ecsWorld.CreateEntityWith(out WeaponComponent _, out LoadedItemConfigComponent initComponent);
            initComponent.ElementRoot = weaponRoot;
        }

        public void Initialize()
        {
            if (_uidDict.EntitiesCount <= 0)
            {
                throw new Exception("UidSystem was not init!");
            }

            var uidDict = _uidDict.Components1[0].UidToEntityDict;
            
            foreach (int i in _loadedWeapons)
            {
                XElement weaponRoot = _loadedWeapons.Components1[i].ElementRoot;
                XElement woundListElement = weaponRoot.GetElement("WeaponWoundList");

                int weaponEntity = _loadedWeapons.Entities[i];
                var randomizer = _ecsWorld.AddComponent<WoundRandomizerComponent>(weaponEntity);
                foreach (XElement woundElement in woundListElement.Elements("WeaponWound"))
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
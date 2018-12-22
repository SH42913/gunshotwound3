using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using GunshotWound2.Armor;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Weapons
{
    [EcsInject]
    public class WeaponInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;

        private const string WEAPON_KEY = "WeaponGroup";
        private const string TYPE_ELEMENT_KEY = "WeaponType";
        private const string HASHES_ELEMENT_KEY = "WeaponHashes";
        private const string BASE_STATS_ELEMENT_KEY = "BaseStats";
        private const string ARMOR_STATS_ELEMENT_KEY = "ArmorStats";

        private readonly GswLogger _logger;

        public WeaponInitSystem()
        {
            _logger = new GswLogger(typeof(WeaponInitSystem));
        }

        public void PreInitialize()
        {
            try
            {
                GenerateWeapons();
            }
            catch (Exception e)
            {
                _logger.MakeLog(e.Message);
            }
        }

        private void GenerateWeapons()
        {
            string fullPath = Environment.CurrentDirectory + GunshotWound2Script.WEAPON_CONFIG_PATH;
            var file = new FileInfo(fullPath);
            if (!file.Exists)
            {
                throw new Exception($"Can\'t find {fullPath}");
            }

            XElement xmlRoot = XDocument.Load(file.OpenRead()).Root;
            if (xmlRoot == null)
            {
                throw new Exception($"Can\'t find root in {fullPath}");
            }

            foreach (XElement weaponRoot in xmlRoot.Elements(WEAPON_KEY))
            {
                CreateWeapon(weaponRoot);
            }
        }

        private void CreateWeapon(XElement weaponRoot)
        {
            XElement weaponTypeElement = weaponRoot.GetElement(TYPE_ELEMENT_KEY);
            XElement weaponHashesElement = weaponRoot.GetElement(HASHES_ELEMENT_KEY);
            XElement baseStatsElement = weaponRoot.GetElement(BASE_STATS_ELEMENT_KEY);
            XElement armorStatsElement = weaponRoot.GetElement(ARMOR_STATS_ELEMENT_KEY);

            int entity = _ecsWorld.CreateEntityWith(out WeaponTypeComponent type, out WeaponInitComponent initComponent);
            initComponent.WeaponRoot = weaponRoot;
            type.Type = weaponTypeElement.GetEnum<WeaponTypes>("Type");

            AttachHashes(weaponHashesElement, entity);
            AttachBaseStats(baseStatsElement, entity);
            AttachArmorStats(armorStatsElement, entity);
        }

        private void AttachHashes(XElement hashesElement, int entity)
        {
            var hashesComponent = _ecsWorld.AddComponent<WeaponHashesComponent>(entity);
            hashesComponent.Name = hashesElement.GetAttributeValue("Name");
#if DEBUG
            _logger.MakeLog($"Loading Weapon {hashesComponent.Name}");
#endif

            var hashStrings = hashesElement.GetAttributeValue("Hashes").Split(';');
            foreach (string hashString in hashStrings)
            {
                if (uint.TryParse(hashString, out var hash))
                {
                    hashesComponent.Hashes.Add(hash);
                }
                else
                {
                    _logger.MakeLog($"Wrong weapon hash: {hashString}");
                }
            }
            
#if DEBUG
            _logger.MakeLog(hashesComponent.ToString());
#endif
        }

        private void AttachBaseStats(XElement statsElement, int entity)
        {
            var stats = _ecsWorld.AddComponent<BaseWeaponStatsComponent>(entity);
            stats.DamageMult = statsElement.GetFloat("DamageMult");
            stats.BleedingMult = statsElement.GetFloat("BleedingMult");
            stats.PainMult = statsElement.GetFloat("PainMult");
            stats.CritChance = statsElement.GetFloat("CritChance");

#if DEBUG
            _logger.MakeLog(stats.ToString());
#endif
        }

        private void AttachArmorStats(XElement statsElement, int entity)
        {
            var stats = _ecsWorld.AddComponent<ArmorWeaponStatsComponent>(entity);
            stats.CanPenetrateArmor = statsElement.GetBool("CanPenetrateArmor");
            stats.ChanceToPenetrateHelmet = statsElement.GetFloat("ChanceToPenetrateHelmet");
            stats.ArmorDamage = statsElement.GetInt("ArmorDamage");
            stats.MinArmorPercentForPenetration = statsElement.GetFloat("MinArmorPercentForPenetration");

#if DEBUG
            _logger.MakeLog(stats.ToString());
#endif
        }

        public void PreDestroy()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using GunshotWound2.Armor;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Weapons
{
    [EcsInject]
    public class WeaponInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;
        
        private const string CONFIG_PATH = "\\Plugins\\GswConfigs\\GswWeaponConfig.xml";
        private const string WEAPON_KEY = "WeaponGroup";
        private const string TYPE_ELEMENT_KEY = "WeaponType";
        private const string HASHES_ELEMENT_KEY = "WeaponHashes";
        private const string BASE_STATS_ELEMENT_KEY = "BaseStats";
        private const string ARMOR_STATS_ELEMENT_KEY = "ArmorStats";
        
        public void PreInitialize()
        {
            try
            {
                GenerateWeapons();
            }
            catch (Exception e)
            {
                Game.Console.Print(e.Message);
            }
        }

        private void GenerateWeapons()
        {
            string fullPath = Environment.CurrentDirectory + CONFIG_PATH;
            var file = new FileInfo(fullPath);
            if (!file.Exists)
            {
                throw new Exception("Can't find " + fullPath);
            }
            
            XElement xmlRoot = XDocument.Load(file.OpenRead()).Root;
            if (xmlRoot == null)
            {
                throw new Exception("Can't find root in " + CONFIG_PATH);
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
            type.Type = (WeaponTypes) Enum.Parse(typeof(WeaponTypes), weaponTypeElement.Attribute("Type").Value);
            
            AttachHashes(weaponHashesElement, entity);
            AttachBaseStats(baseStatsElement, entity);
            AttachArmorStats(armorStatsElement, entity);
        }

        private void AttachHashes(XElement hashesElement, int entity)
        {
            var hashesComponent = _ecsWorld.AddComponent<WeaponHashesComponent>(entity);
            hashesComponent.Name = hashesElement.Attribute("Name").Value;
#if DEBUG
            Game.Console.Print("Loading Weapon " + hashesComponent.Name);
#endif
            
            var hashes = new List<uint>();
            foreach (string hashString in hashesElement.Attribute("Hashes").Value.Split(';'))
            {
                if (uint.TryParse(hashString, out var hash))
                {
                    hashes.Add(hash);
#if DEBUG
                    Game.Console.Print("Added Hash " + hash);
#endif
                }
                else
                {
                    Game.Console.Print("Wrong weapon hash: " + hashString);
                }
            }
            hashesComponent.Hashes = hashes;
        }

        private void AttachBaseStats(XElement statsElement, int entity)
        {
            var stats = _ecsWorld.AddComponent<BaseWeaponStatsComponent>(entity);
            stats.DamageMult = float.Parse(statsElement.Attribute("DamageMult").Value, CultureInfo.InvariantCulture);
            stats.BleedingMult = float.Parse(statsElement.Attribute("BleedingMult").Value, CultureInfo.InvariantCulture);
            stats.PainMult = float.Parse(statsElement.Attribute("PainMult").Value, CultureInfo.InvariantCulture);
            stats.CritChance = float.Parse(statsElement.Attribute("CritChance").Value, CultureInfo.InvariantCulture);

#if DEBUG
            Game.Console.Print(stats.ToString());
#endif
        }

        private void AttachArmorStats(XElement statsElement, int entity)
        {
            var stats = _ecsWorld.AddComponent<ArmorWeaponStatsComponent>(entity);
            stats.CanPenetrateArmor = bool.Parse(statsElement.Attribute("CanPenetrateArmor").Value);
            stats.ChanceToPenetrateHelmet = float.Parse(statsElement.Attribute("ChanceToPenetrateHelmet").Value, CultureInfo.InvariantCulture);
            stats.ArmorDamage = int.Parse(statsElement.Attribute("ArmorDamage").Value);
            stats.MinArmorPercentForPenetration = float.Parse(statsElement.Attribute("MinArmorPercentForPenetration").Value, CultureInfo.InvariantCulture);
            
#if DEBUG
            Game.Console.Print(stats.ToString());
#endif
        }

        public void PreDestroy()
        {
        }
    }
}
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

            foreach (XElement weaponRoot in xmlRoot.Elements("WeaponGroup"))
            {
                CreateWeapon(weaponRoot);
            }
        }

        private void CreateWeapon(XElement weaponRoot)
        {
            XElement weaponTypeElement = weaponRoot.GetElement("WeaponType");
            XElement hashesElement = weaponRoot.GetElement("WeaponHashes");
            XElement baseStatsElement = weaponRoot.GetElement("BaseStats");

            int entity = _ecsWorld.CreateEntityWith(out WeaponTypeComponent type, out InitElementComponent initComponent);
            initComponent.ElementRoot = weaponRoot;
            type.Type = weaponTypeElement.GetEnum<WeaponTypes>("Type");
            
            var hashesComponent = _ecsWorld.AddComponent<HashesComponent>(entity);
            hashesComponent.FillHashesComponent(hashesElement, _logger);

            AttachBaseStats(baseStatsElement, entity);
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

        public void PreDestroy()
        {
        }
    }
}
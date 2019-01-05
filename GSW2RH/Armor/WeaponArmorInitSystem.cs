using System.Xml.Linq;
using GunshotWound2.Utils;
using GunshotWound2.Weapons;
using Leopotam.Ecs;

namespace GunshotWound2.Armor
{
    [EcsInject]
    public class WeaponArmorInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<WeaponTypeComponent, InitElementComponent, HashesComponent> _initWeapons;

        private readonly GswLogger _logger;

        public WeaponArmorInitSystem()
        {
            _logger = new GswLogger(typeof(WeaponArmorInitSystem));
        }

        public void Initialize()
        {
            foreach (int i in _initWeapons)
            {
                XElement weaponRoot = _initWeapons.Components2[i].ElementRoot;
                XElement statsElement = weaponRoot.GetElement("ArmorStats");
                int weaponEntity = _initWeapons.Entities[i];

                var stats = _ecsWorld.AddComponent<ArmorWeaponStatsComponent>(weaponEntity);
                stats.CanPenetrateArmor = statsElement.GetBool("CanPenetrateArmor");
                stats.ChanceToPenetrateHelmet = statsElement.GetFloat("ChanceToPenetrateHelmet");
                stats.ArmorDamage = statsElement.GetInt("ArmorDamage");
                stats.MinArmorPercentForPenetration = statsElement.GetFloat("MinArmorPercentForPenetration");

#if DEBUG
                string name = _initWeapons.Components3[i].Name;
                _logger.MakeLog($"Weapon {name} got {stats}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
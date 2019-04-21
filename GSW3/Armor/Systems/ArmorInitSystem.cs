using System.Xml.Linq;
using GSW3.BodyParts;
using GSW3.Configs;
using GSW3.GswWorld;
using GSW3.Utils;
using GSW3.Weapons;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Armor.Systems
{
    [EcsInject]
    public class ArmorInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<LoadedItemConfigComponent, WeaponComponent> _initWeapons = null;
        private readonly EcsFilter<LoadedItemConfigComponent, BodyPartComponent> _initParts = null;
        private readonly EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newPeds = null;

#if DEBUG
        private readonly GswLogger _logger;

        public ArmorInitSystem()
        {
            _logger = new GswLogger(typeof(ArmorInitSystem));
        }
#endif

        public void Initialize()
        {
            foreach (int i in _initWeapons)
            {
                XElement weaponRoot = _initWeapons.Components1[i].ElementRoot;
                XElement statsElement = weaponRoot.GetElement("WeaponArmorStats");
                EcsEntity weaponEntity = _initWeapons.Entities[i];

                var stats = _ecsWorld.AddComponent<ArmorWeaponStatsComponent>(weaponEntity);
                stats.CanPenetrateArmor = statsElement.GetBool("CanPenetrateArmor");
                stats.ChanceToPenetrateHelmet = statsElement.GetFloat("ChanceToPenetrateHelmet");
                stats.ArmorDamage = statsElement.GetInt("ArmorDamage");
                stats.MinArmorPercentForPenetration = statsElement.GetFloat("MinArmorPercentForPenetration");
            }

            foreach (int i in _initParts)
            {
                EcsEntity entity = _initParts.Entities[i];
                XElement partRoot = _initParts.Components1[i].ElementRoot;
                XElement protection = partRoot.Element("Protection");

                var bodyArmor = _ecsWorld.AddComponent<BodyPartArmorComponent>(entity);
                bodyArmor.ProtectedByHelmet = protection.GetBool("ByHelmet");
                bodyArmor.ProtectedByBodyArmor = protection.GetBool("ByArmor");
            }
        }

        public void Run()
        {
            foreach (int i in _newPeds)
            {
                Ped ped = _newPeds.Components1[i].ThisPed;
                EcsEntity pedEntity = _newPeds.Entities[i];

                var armor = _ecsWorld.AddComponent<PedArmorComponent>(pedEntity);
                armor.Armor = ped.Armor;
            }
        }

        public void Destroy()
        {
        }
    }
}
using System.Xml.Linq;
using GunshotWound2.Bodies;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.Weapons;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Armor.Systems
{
    [EcsInject]
    public class ArmorInitSystem : IEcsInitSystem,  IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<WeaponTypeComponent, InitElementComponent, HashesComponent> _initWeapons;
        private EcsFilter<BodyPartComponent, InitElementComponent, HashesComponent> _initParts;

        private EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newPeds;

        private readonly GswLogger _logger;

        public ArmorInitSystem()
        {
            _logger = new GswLogger(typeof(ArmorInitSystem));
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
            
            foreach (int i in _initParts)
            {
                int entity = _initParts.Entities[i];
                XElement partRoot = _initParts.Components2[i].ElementRoot;
                XElement protection = partRoot.Element("Protection");

                var bodyArmor = _ecsWorld.AddComponent<BodyPartArmorComponent>(entity);
                bodyArmor.ProtectedByHelmet = protection.GetBool("ByHelmet");
                bodyArmor.ProtectedByBodyArmor = protection.GetBool("ByArmor");

#if DEBUG
                string partName = _initParts.Components3[i].Name;
                _logger.MakeLog($"BodyPart {partName} got {bodyArmor}");
#endif
            }
        }
        
        public void Run()
        {
            foreach (int i in _newPeds)
            {
                Ped ped = _newPeds.Components1[i].ThisPed;
                int pedEntity = _newPeds.Entities[i];

                var armor = _ecsWorld.AddComponent<ArmorComponent>(pedEntity);
                armor.Armor = ped.Armor;
            }
        }

        public void Destroy()
        {
        }
    }
}
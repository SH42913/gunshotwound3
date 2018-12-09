using GunshotWound2.GswWorld;
using GunshotWound2.HitDetecting;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Weapons.FireArms
{
    [EcsInject]
    public class FireArmsWoundSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, DamagedBodyPartComponent, DamagedByWeaponComponent> _damagedPeds;

        private readonly GswLogger _logger;

        public FireArmsWoundSystem()
        {
            _logger = new GswLogger(typeof(FireArmsWoundSystem));
        }

        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                DamagedByWeaponComponent damagedWeapon = _damagedPeds.Components3[i];
                if (damagedWeapon.WeaponType != WeaponTypes.FIRE_ARMS) continue;

                Ped ped = _damagedPeds.Components1[i].ThisPed;
                int weaponEntity = _damagedPeds.Components3[i].WeaponEntity;
                int pedEntity = _damagedPeds.Entities[i];
                var woundRandomizer = _ecsWorld.GetComponent<FireArmsWoundRandomizerComponent>(weaponEntity);
                if (woundRandomizer == null)
                {
                    _logger.MakeLog("Weapon Entity doesn't have " + nameof(FireArmsWoundRandomizerComponent));
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

                FireArmsWounds wound = woundRandomizer.WoundRandomizer.NextWithReplacement();
                switch (wound)
                {
                    case FireArmsWounds.GRAZE_WOUND:
                        _logger.MakeLog("Ped " + ped.Name() + " have got graze GSW");
                        break;
                    case FireArmsWounds.FLESH_WOUND:
                        _logger.MakeLog("Ped " + ped.Name() + " have got flesh GSW");
                        break;
                    case FireArmsWounds.PENETRATING_WOUND:
                        _logger.MakeLog("Ped " + ped.Name() + " have got penetrating GSW");
                        break;
                    case FireArmsWounds.PERFORATING_WOUND:
                        _logger.MakeLog("Ped " + ped.Name() + " have got perforating GSW");
                        break;
                    case FireArmsWounds.AVULSIVE_WOUND:
                        _logger.MakeLog("Ped " + ped.Name() + " have got avulsive GSW");
                        break;
                    default:
                        _logger.MakeLog("Ped " + ped.Name() + " have got Unknow wound");
                        continue;
                }
            }
        }
    }
}
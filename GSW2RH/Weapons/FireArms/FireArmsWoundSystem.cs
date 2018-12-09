using System;
using GunshotWound2.GswWorld;
using GunshotWound2.HitDetecting;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Weapons.FireArms
{
    [EcsInject]
    public class FireArmsWoundSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, DamagedBodyPartComponent, DamagedByWeaponComponent> _damagedPeds;
        
        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                DamagedByWeaponComponent damagedWeapon = _damagedPeds.Components3[i];
                if(damagedWeapon.WeaponType != WeaponTypes.FIRE_ARMS) continue;

                Ped ped = _damagedPeds.Components1[i].ThisPed;
                int weaponEntity = _damagedPeds.Components3[i].WeaponEntity;
                int pedEntity = _damagedPeds.Entities[i];
                var woundRandomizer = _ecsWorld.GetComponent<FireArmsWoundRandomizerComponent>(weaponEntity);
                if (woundRandomizer == null)
                {
                    Game.Console.Print("Weapon Entity doesn't have " + nameof(FireArmsWoundRandomizerComponent));
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

                FireArmsWounds wound = woundRandomizer.WoundRandomizer.NextWithReplacement();
                switch (wound)
                {
                    case FireArmsWounds.GRAZE_WOUND:
                        Game.Console.Print("Ped have got graze GSW");
                        break;
                    case FireArmsWounds.FLESH_WOUND:
                        Game.Console.Print("Ped have got flesh GSW");
                        break;
                    case FireArmsWounds.PENETRATING_WOUND:
                        Game.Console.Print("Ped have got penetrating GSW");
                        break;
                    case FireArmsWounds.PERFORATING_WOUND:
                        Game.Console.Print("Ped have got perforating GSW");
                        break;
                    case FireArmsWounds.AVULSIVE_WOUND:
                        Game.Console.Print("Ped have got avulsive GSW");
                        break;
                    default:
                        Game.Console.Print("Unknow wound");
                        continue;
                }
            }
        }
    }
}
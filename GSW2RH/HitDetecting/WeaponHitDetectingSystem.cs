using GunshotWound2.GswWorld;
using GunshotWound2.Weapons;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.HitDetecting
{
    [EcsInject]
    public class WeaponHitDetectingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, HasBeenHitMarkComponent, DamagedBodyPartComponent> _damagedPeds;
        private EcsFilter<WeaponHashesComponent, WeaponTypeComponent> _weaponGroups;
        
        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                Ped ped = _damagedPeds.Components1[i].ThisPed;
                if(!ped.Exists()) continue;

                DetectWeaponHash(ped, _damagedPeds.Entities[i]);
            }
        }

        private void DetectWeaponHash(Ped ped, int pedEntity)
        {
            foreach (int i in _weaponGroups)
            {
                WeaponHashesComponent hashesComponent = _weaponGroups.Components1[i];

                foreach (uint hash in hashesComponent.Hashes)
                {
                    if(!NativeFunction.Natives.HAS_PED_BEEN_DAMAGED_BY_WEAPON<bool>(ped, hash, 0)) continue;

#if DEBUG
                    Game.Console.Print("Ped " + ped.Model.Name + " was damaged by " + hashesComponent.Name);
#endif
                    var damaged = _ecsWorld.AddComponent<DamagedByWeaponComponent>(pedEntity);
                    damaged.WeaponEntity = _weaponGroups.Entities[i];
                    damaged.WeaponType = _weaponGroups.Components2[i].Type;
                    return;
                }
            }
            
            Game.Console.Print("!!!Ped " + ped.Model.Name + " was damaged by UNKNOWN weapon!!!");
        }
    }
}
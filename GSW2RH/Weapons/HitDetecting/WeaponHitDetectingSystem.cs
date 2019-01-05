using GunshotWound2.BaseHitDetecting;
using GunshotWound2.Bodies;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Weapons.HitDetecting
{
    [EcsInject]
    public class WeaponHitDetectingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, HasBeenHitMarkComponent, DamagedBodyPartComponent> _damagedPeds;
        private EcsFilter<HashesComponent, WeaponTypeComponent> _weaponGroups;

        private readonly GswLogger _logger;

        public WeaponHitDetectingSystem()
        {
            _logger = new GswLogger(typeof(WeaponHitDetectingSystem));
        }

        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                Ped ped = _damagedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                DetectWeaponHash(ped, _damagedPeds.Entities[i]);
            }
        }

        private void DetectWeaponHash(Ped ped, int pedEntity)
        {
            foreach (int i in _weaponGroups)
            {
                HashesComponent hashesComponent = _weaponGroups.Components1[i];

                foreach (uint hash in hashesComponent.Hashes)
                {
                    if (!NativeFunction.Natives.HAS_PED_BEEN_DAMAGED_BY_WEAPON<bool>(ped, hash, 0)) continue;

#if DEBUG
                    _logger.MakeLog($"Ped {ped.Name(pedEntity)} was damaged by {hashesComponent.Name}");
#endif
                    var damaged = _ecsWorld.AddComponent<DamagedByWeaponComponent>(pedEntity);
                    damaged.WeaponEntity = _weaponGroups.Entities[i];
                    damaged.WeaponType = _weaponGroups.Components2[i].Type;
                    return;
                }
            }

            _logger.MakeLog($"!!!Ped {ped.Name(pedEntity)} was damaged by UNKNOWN weapon!!!");
        }
    }
}
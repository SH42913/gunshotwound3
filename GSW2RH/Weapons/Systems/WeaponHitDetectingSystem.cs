using GunshotWound2.BaseHitDetecting;
using GunshotWound2.BodyParts;
using GunshotWound2.GswWorld;
using GunshotWound2.Hashes;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Weapons.Systems
{
    [EcsInject]
    public class WeaponHitDetectingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, HasBeenHitMarkComponent, DamagedBodyPartComponent> _damagedPeds;
        private EcsFilter<HashesComponent, WeaponComponent> _weapons;

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
            foreach (int i in _weapons)
            {
                HashesComponent hashesComponent = _weapons.Components1[i];

                foreach (uint hash in hashesComponent.Hashes)
                {
                    if (!NativeFunction.Natives.HAS_PED_BEEN_DAMAGED_BY_WEAPON<bool>(ped, hash, 0)) continue;

                    int weaponEntity = _weapons.Entities[i];
#if DEBUG
                    string weaponName = weaponEntity.GetEntityName();
                    _logger.MakeLog($"Ped {ped.Name(pedEntity)} was damaged by {weaponName}");
#endif

                    var damaged = _ecsWorld.AddComponent<DamagedByWeaponComponent>(pedEntity);
                    damaged.WeaponEntity = weaponEntity;
                    return;
                }
            }

            _logger.MakeLog($"!!!Ped {ped.Name(pedEntity)} was damaged by UNKNOWN weapon");
        }
    }
}
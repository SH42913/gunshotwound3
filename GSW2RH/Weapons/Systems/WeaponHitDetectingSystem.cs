using GunshotWound2.BaseHitDetecting;
using GunshotWound2.Bodies;
using GunshotWound2.GswWorld;
using GunshotWound2.Hashes;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
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
        private EcsFilter<HashesComponent, WeaponComponent, WoundRandomizerComponent> _weapons;

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
                    int woundEntity = _weapons.Components3[i].WoundRandomizer.NextWithReplacement();
#if DEBUG
                    string weaponName = weaponEntity.GetEntityName(_ecsWorld);
                    string woundName = woundEntity.GetEntityName(_ecsWorld);
                    _logger.MakeLog($"Ped {ped.Name(pedEntity)} was damaged by {weaponName} with wound {woundName}");
#endif
                    
                    var damaged = _ecsWorld.AddComponent<DamagedByWeaponComponent>(pedEntity);
                    damaged.WeaponEntity = weaponEntity;
                    damaged.WoundEntity = woundEntity;

                    var wounded = _ecsWorld.EnsureComponent<WoundedComponent>(pedEntity, out bool _);
                    wounded.WoundEntities.Add(woundEntity);
                    return;
                }
            }

            _logger.MakeLog($"!!!Ped {ped.Name(pedEntity)} was damaged by UNKNOWN weapon");
        }
    }
}
﻿using GSW3.BaseHitDetecting;
using GSW3.BodyParts;
using GSW3.GswWorld;
using GSW3.Hashes;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GSW3.Weapons.Systems
{
    [EcsInject]
    public class WeaponHitDetectingSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<GswPedComponent, HasBeenHitMarkComponent, DamagedBodyPartComponent> _damaged = null;
        private readonly EcsFilter<HashesComponent, WeaponComponent> _weapons = null;

        private readonly GswLogger _logger;

        public WeaponHitDetectingSystem()
        {
            _logger = new GswLogger(typeof(WeaponHitDetectingSystem));
        }

        public void Run()
        {
            foreach (int i in _damaged)
            {
                Ped ped = _damaged.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                DetectWeaponHash(ped, _damaged.Entities[i]);
            }
        }

        private void DetectWeaponHash(Ped ped, EcsEntity pedEntity)
        {
            foreach (int i in _weapons)
            {
                HashesComponent hashesComponent = _weapons.Components1[i];
                foreach (uint hash in hashesComponent.Hashes)
                {
                    if (!NativeFunction.Natives.HAS_PED_BEEN_DAMAGED_BY_WEAPON<bool>(ped, hash, 0)) continue;

                    EcsEntity weaponEntity = _weapons.Entities[i];
#if DEBUG
                    _logger.MakeLog($"{pedEntity.GetEntityName()} was damaged by {weaponEntity.GetEntityName()}({hash})");
#endif

                    var damaged = _ecsWorld.AddComponent<DamagedByWeaponComponent>(pedEntity);
                    damaged.WeaponEntity = weaponEntity;
                    return;
                }
            }

            _logger.MakeLog($"!!!WARNING!!! Ped {pedEntity.GetEntityName()} was damaged by UNKNOWN weapon");
        }
    }
}
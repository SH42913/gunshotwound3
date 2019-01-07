using System;
using GunshotWound2.Bodies;
using GunshotWound2.Utils;
using GunshotWound2.Weapons;
using GunshotWound2.Wounds;
using Leopotam.Ecs;

namespace GunshotWound2.Crits
{
    [EcsInject]
    public class CritSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<DamagedByWeaponComponent, DamagedBodyPartComponent, WoundedComponent> _woundedPeds;

        private readonly GswLogger _logger;
        private static readonly Random Random = new Random();

        public CritSystem()
        {
            _logger = new GswLogger(typeof(CritSystem));
        }

        public void Run()
        {
            foreach (int i in _woundedPeds)
            {
                DamagedByWeaponComponent damagedByWeapon = _woundedPeds.Components1[i];
                WoundedComponent wounded = _woundedPeds.Components3[i];
                int pedEntity = _woundedPeds.Entities[i];

                int bodyPartEntity = _woundedPeds.Components2[i].DamagedBodyPartEntity;
                var bodyPartCrits = _ecsWorld.GetComponent<WoundRandomizerComponent>(bodyPartEntity);
                if (bodyPartCrits == null)
                {
#if DEBUG
                    string partName = bodyPartEntity.GetEntityName(_ecsWorld);
                    _logger.MakeLog($"BodyPart {partName} doesn't have crits");
#endif
                    continue;
                }

                int weaponEntity = damagedByWeapon.WeaponEntity;
                var weaponChanceComponent = _ecsWorld.GetComponent<CritChanceComponent>(weaponEntity);
                float weaponChance = weaponChanceComponent?.CritChance ?? 0f;

                int woundEntity = damagedByWeapon.WoundEntity;
                var woundChanceComponent = _ecsWorld.GetComponent<CritChanceComponent>(woundEntity);
                float woundChance = woundChanceComponent?.CritChance ?? 0f;

                var bodyPartChanceComponent = _ecsWorld.GetComponent<CritChanceComponent>(bodyPartEntity);
                float bodyPartChance = bodyPartChanceComponent?.CritChance ?? 0f;

                if (weaponChance <= 0 || woundChance <= 0 || bodyPartChance <= 0)
                {
#if DEBUG
                    _logger.MakeLog($"One of CritChances below 0! WP{weaponChance}/WO{woundChance}/BP{bodyPartChance}");
#endif
                    continue;
                }

                float finalChance = weaponChance * woundChance * bodyPartChance;
                bool critSuccess = Random.IsTrueWithProbability(finalChance);
#if DEBUG
                _logger.MakeLog($"Entity ({pedEntity}) crit check is {critSuccess}, when chance was {finalChance}" +
                                $"(WP{weaponChance}/WO{woundChance}/BP{bodyPartChance})");
#endif
                if(!critSuccess) continue;

                int critEntity = bodyPartCrits.WoundRandomizer.NextWithReplacement();
                wounded.WoundEntities.Add(critEntity);
#if DEBUG
                _logger.MakeLog($"Entity ({pedEntity}) have got crit {critEntity.GetEntityName(_ecsWorld)}");
#endif
            }
        }
    }
}
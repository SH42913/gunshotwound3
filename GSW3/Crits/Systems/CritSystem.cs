using System;
using GSW3.BodyParts;
using GSW3.Health;
using GSW3.Utils;
using GSW3.Weapons;
using GSW3.Wounds;
using Leopotam.Ecs;

namespace GSW3.Crits.Systems
{
    [EcsInject]
    public class CritSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly Random _random = null;
        private readonly EcsFilter<FullyHealedComponent, CritListComponent> _healedPeds = null;

        private readonly EcsFilter<
            DamagedByWeaponComponent,
            DamagedBodyPartComponent,
            WoundedComponent,
            CritListComponent> _woundedPeds = null;

#if DEBUG
        private readonly GswLogger _logger;

        public CritSystem()
        {
            _logger = new GswLogger(typeof(CritSystem));
        }
#endif

        public void Run()
        {
            foreach (int i in _healedPeds)
            {
                _healedPeds.Components2[i].CritList.Clear();
            }

            foreach (int i in _woundedPeds)
            {
                DamagedByWeaponComponent damagedByWeapon = _woundedPeds.Components1[i];
                WoundedComponent wounded = _woundedPeds.Components3[i];
                CritListComponent critList = _woundedPeds.Components4[i];

                EcsEntity bodyPartEntity = _woundedPeds.Components2[i].DamagedBodyPartEntity;
                var bodyPartCrits = _ecsWorld.GetComponent<WoundRandomizerComponent>(bodyPartEntity);
                if (bodyPartCrits == null)
                {
#if DEBUG
                    _logger.MakeLog($"BodyPart {bodyPartEntity.GetEntityName()} doesn't have crits");
#endif
                    continue;
                }

                EcsEntity weaponEntity = damagedByWeapon.WeaponEntity;
                var weaponChanceComponent = _ecsWorld.GetComponent<CritChanceComponent>(weaponEntity);
                float weaponChance = weaponChanceComponent?.CritChance ?? 0f;

                EcsEntity woundEntity = damagedByWeapon.WoundEntity;
                var woundChanceComponent = _ecsWorld.GetComponent<CritChanceComponent>(woundEntity);
                float woundChance = woundChanceComponent?.CritChance ?? 0f;

                var bodyPartChanceComponent = _ecsWorld.GetComponent<CritChanceComponent>(bodyPartEntity);
                float bodyPartChance = bodyPartChanceComponent?.CritChance ?? 0f;

                if (weaponChance <= 0 || woundChance <= 0 || bodyPartChance <= 0)
                {
#if DEBUG
                    _logger.MakeLog("One of CritChances below 0! " +
                                    $"(WP {weaponChance:0.00}/" +
                                    $"WO {woundChance:0.00}/" +
                                    $"BP {bodyPartChance:0.00})");
#endif
                    continue;
                }

                float finalChance = weaponChance * woundChance * bodyPartChance;
                bool critSuccess = _random.IsTrueWithProbability(finalChance);
#if DEBUG
                EcsEntity pedEntity = _woundedPeds.Entities[i];
                _logger.MakeLog(
                    $"{pedEntity.GetEntityName()} crit check is {critSuccess}, when chance was {finalChance:0.000}" +
                    $"(WP {weaponChance:0.00}/" +
                    $"WO {woundChance:0.00}/" +
                    $"BP {bodyPartChance:0.00})");
#endif
                if (!critSuccess) continue;

                EcsEntity critEntity = bodyPartCrits.WoundRandomizer.NextWithReplacement();
                wounded.WoundEntities.Add(critEntity);
                critList.CritList.Add(critEntity);
#if DEBUG
                _logger.MakeLog($"{pedEntity.GetEntityName()} have got crit {critEntity.GetEntityName()}");
#endif
            }
        }
    }
}
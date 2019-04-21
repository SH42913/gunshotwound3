using System;
using GSW3.BodyParts;
using GSW3.GswWorld;
using GSW3.Utils;
using GSW3.Weapons;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Armor.Systems
{
    [EcsInject]
    public class HelmetHitProcessingSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly Random _random = null;

        private readonly EcsFilter<
            GswPedComponent,
            DamagedBodyPartComponent,
            DamagedByWeaponComponent,
            PedArmorComponent> _damagedPeds = null;

#if DEBUG
        private readonly GswLogger _logger;

        public HelmetHitProcessingSystem()
        {
            _logger = new GswLogger(typeof(HelmetHitProcessingSystem));
        }
#endif

        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                Ped ped = _damagedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                EcsEntity pedEntity = _damagedPeds.Entities[i];
                if (!ped.IsWearingHelmet)
                {
#if DEBUG
                    _logger.MakeLog($"{pedEntity.GetEntityName()} doesn't have helmet");
#endif
                    continue;
                }

                EcsEntity bodyPartEntity = _damagedPeds.Components2[i].DamagedBodyPartEntity;
                var bodyArmor = _ecsWorld.GetComponent<BodyPartArmorComponent>(bodyPartEntity);
                if (bodyArmor == null || !bodyArmor.ProtectedByHelmet)
                {
#if DEBUG
                    string partName = bodyPartEntity.GetEntityName();
                    _logger.MakeLog($"Helmet of {pedEntity.GetEntityName()} doesn't protect {partName}");
#endif
                    continue;
                }

                EcsEntity weaponEntity = _damagedPeds.Components3[i].WeaponEntity;
                var weaponStats = _ecsWorld.GetComponent<ArmorWeaponStatsComponent>(weaponEntity);
                if (weaponStats == null)
                {
#if DEBUG
                    _logger.MakeLog($"Weapon {weaponEntity.GetEntityName()} doesn't have {nameof(ArmorWeaponStatsComponent)}");
#endif
                    continue;
                }

                float chance = weaponStats.ChanceToPenetrateHelmet;
                bool helmetPenetrated = _random.IsTrueWithProbability(chance);
                if (!helmetPenetrated)
                {
#if DEBUG
                    _logger.MakeLog($"Helmet of {pedEntity.GetEntityName()} was not penetrated, " +
                                    $"when chance was {chance:0.00}");
#endif
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    _ecsWorld.RemoveComponent<DamagedBodyPartComponent>(pedEntity);
                    continue;
                }

#if DEBUG
                _logger.MakeLog($"Helmet of {pedEntity.GetEntityName()} was penetrated, when chance was {chance:0.00}");
#endif
            }
        }
    }
}
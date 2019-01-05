using System;
using GunshotWound2.BaseHitDetecting;
using GunshotWound2.Bodies;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.Weapons.HitDetecting;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Armor
{
    [EcsInject]
    public class HelmetHitProcessingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, DamagedBodyPartComponent, DamagedByWeaponComponent, ArmorComponent> _damagedPeds;

        private static readonly Random Random = new Random();
        private readonly GswLogger _logger;

        public HelmetHitProcessingSystem()
        {
            _logger = new GswLogger(typeof(HelmetHitProcessingSystem));
        }

        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                GswPedComponent gswPed = _damagedPeds.Components1[i];
                Ped ped = gswPed.ThisPed;
                if (!ped.Exists()) continue;
                
                int pedEntity = _damagedPeds.Entities[i];
                if (!ped.IsWearingHelmet)
                {
#if DEBUG
                    _logger.MakeLog($"Ped {ped.Name(pedEntity)} doesn\'t have Helmet");
#endif
                    continue;
                }

                int bodyPartEntity = _damagedPeds.Components2[i].DamagedBodyPartEntity;
                var bodyArmor = _ecsWorld.GetComponent<BodyPartArmorComponent>(bodyPartEntity);
                if (bodyArmor == null || !bodyArmor.ProtectedByHelmet)
                {
#if DEBUG
                    string partName = _ecsWorld.GetComponent<HashesComponent>(bodyPartEntity).Name;
                    _logger.MakeLog($"Helmet of {ped.Name(pedEntity)} doesn\'t protect {partName}");
#endif
                    continue;
                }

                int weaponEntity = _damagedPeds.Components3[i].WeaponEntity;
                var weaponStats = _ecsWorld.GetComponent<ArmorWeaponStatsComponent>(weaponEntity);
                if (weaponStats == null)
                {
#if DEBUG
                    _logger.MakeLog($"This weapon doesn\'t have {nameof(ArmorWeaponStatsComponent)}");
#endif
                    continue;
                }

                float chance = weaponStats.ChanceToPenetrateHelmet;
                bool helmetPenetrated = Random.IsTrueWithProbability(chance);
                if (!helmetPenetrated)
                {
#if DEBUG
                    _logger.MakeLog($"Helmet of {ped.Name(pedEntity)} was not penetrated, when chance was {chance}");
#endif
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

#if DEBUG
                _logger.MakeLog($"Helmet of {ped.Name(pedEntity)} was penetrated, when chance was {chance}");
#endif
            }
        }
    }
}
using System;
using GunshotWound2.GswWorld;
using GunshotWound2.HitDetecting;
using GunshotWound2.Utils;
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
                if (!ped.IsWearingHelmet)
                {
#if DEBUG
                    _logger.MakeLog($"Ped {ped.Name()} doesn\'t have Helmet");
#endif
                    continue;
                }

                BodyParts bodyPart = _damagedPeds.Components2[i].DamagedBodyPart;
                if (bodyPart != BodyParts.HEAD)
                {
#if DEBUG
                    _logger.MakeLog($"Helmet of {ped.Name()} doesn\'t protect {bodyPart}");
#endif
                    continue;
                }

                int weaponEntity = _damagedPeds.Components3[i].WeaponEntity;
                int pedEntity = _damagedPeds.Entities[i];
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
                    _logger.MakeLog($"Helmet of {ped.Name()} was not penetrated, when chance was {chance}");
#endif
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

#if DEBUG
                _logger.MakeLog($"Helmet of {ped.Name()} was penetrated, when chance was {chance}");
#endif
            }
        }
    }
}
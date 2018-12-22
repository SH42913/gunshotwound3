using GunshotWound2.GswWorld;
using GunshotWound2.HitDetecting;
using GunshotWound2.Utils;
using GunshotWound2.WoundProcessing.Health;
using GunshotWound2.WoundProcessing.Pain;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Weapons.FireArms
{
    [EcsInject]
    public class FireArmsWoundSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, DamagedBodyPartComponent, DamagedByWeaponComponent> _damagedPeds;

        private readonly GswLogger _logger;

        public FireArmsWoundSystem()
        {
            _logger = new GswLogger(typeof(FireArmsWoundSystem));
        }

        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                DamagedByWeaponComponent damagedWeapon = _damagedPeds.Components3[i];
                if (damagedWeapon.WeaponType != WeaponTypes.FIRE_ARMS) continue;

                Ped ped = _damagedPeds.Components1[i].ThisPed;
                int weaponEntity = _damagedPeds.Components3[i].WeaponEntity;
                int pedEntity = _damagedPeds.Entities[i];

                var woundRandomizer = _ecsWorld.GetComponent<FireArmsWoundRandomizerComponent>(weaponEntity);
                if (woundRandomizer == null)
                {
                    _logger.MakeLog($"Weapon Entity doesn\'t have {nameof(FireArmsWoundRandomizerComponent)}");
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

                var baseStats = _ecsWorld.GetComponent<BaseWeaponStatsComponent>(weaponEntity);
                if (baseStats == null)
                {
                    _logger.MakeLog($"Weapon Entity doesn\'t have {nameof(BaseWeaponStatsComponent)}");
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

                FireArmsWounds wound = woundRandomizer.WoundRandomizer.NextWithReplacement();
                float damageMult = baseStats.DamageMult;
                float damageAmount;

                float painMult = baseStats.PainMult;
                float painAmount;

                _logger.MakeLog($"Ped {ped.Name(pedEntity)} have got {wound}");
                switch (wound)
                {
                    case FireArmsWounds.GRAZE_WOUND:
                        damageAmount = 5;
                        painAmount = 10;
                        break;
                    case FireArmsWounds.FLESH_WOUND:
                        damageAmount = 7;
                        painAmount = 15;
                        break;
                    case FireArmsWounds.PENETRATING_WOUND:
                        damageAmount = 10;
                        painAmount = 20;
                        break;
                    case FireArmsWounds.PERFORATING_WOUND:
                        damageAmount = 10;
                        painAmount = 20;
                        break;
                    case FireArmsWounds.AVULSIVE_WOUND:
                        damageAmount = 15;
                        painAmount = 30;
                        break;
                    default:
                        continue;
                }

                var newDamage = _ecsWorld.EnsureComponent<ReceivedDamageComponent>(pedEntity, out bool damageIsNew);
                var damage = damageMult * damageAmount;
                if (damageIsNew)
                {
                    newDamage.Damage = damage;
                }
                else
                {
                    newDamage.Damage += damage;
                }
#if DEBUG
                _logger.MakeLog($"Damage {damage} to ped {ped.Name(pedEntity)}");
#endif

                var newPain = _ecsWorld.EnsureComponent<ReceivedPainComponent>(pedEntity, out bool painIsNew);
                var pain = painMult * painAmount;
                if (painIsNew)
                {
                    newPain.Pain = pain;
                }
                else
                {
                    newPain.Pain += pain;
                }
#if DEBUG
                _logger.MakeLog($"Pain {pain} to ped {ped.Name(pedEntity)}");
#endif
            }
        }
    }
}
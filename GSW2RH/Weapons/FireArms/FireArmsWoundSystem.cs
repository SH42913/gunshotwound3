using GunshotWound2.Bodies;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.Weapons.HitDetecting;
using GunshotWound2.WoundProcessing.Bleeding;
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

                FireArmsWounds wound = woundRandomizer.WoundRandomizer.NextWithReplacement();

                var damageMultComponent = _ecsWorld.GetComponent<DamageMultComponent>(weaponEntity);
                float damageMult = damageMultComponent?.Multiplier ?? 1f;
                float damageAmount;

                var painMultComponent = _ecsWorld.GetComponent<BleedingMultComponent>(weaponEntity);
                float painMult = painMultComponent?.Multiplier ?? 1f;
                float painAmount;

                var bleedingMultComponent = _ecsWorld.GetComponent<BleedingMultComponent>(weaponEntity);
                float bleedingMult = bleedingMultComponent?.Multiplier ?? 1f;
                float bleedingAmount;
                
                switch (wound)
                {
                    case FireArmsWounds.GRAZE_WOUND:
                        damageAmount = 5;
                        painAmount = 10;
                        bleedingAmount = 0.2f;
                        break;
                    case FireArmsWounds.FLESH_WOUND:
                        damageAmount = 7;
                        painAmount = 15;
                        bleedingAmount = 0.4f;
                        break;
                    case FireArmsWounds.PENETRATING_WOUND:
                        damageAmount = 9;
                        painAmount = 25;
                        bleedingAmount = 0.6f;
                        break;
                    case FireArmsWounds.PERFORATING_WOUND:
                        damageAmount = 9;
                        painAmount = 20;
                        bleedingAmount = 0.8f;
                        break;
                    case FireArmsWounds.AVULSIVE_WOUND:
                        damageAmount = 11;
                        painAmount = 30;
                        bleedingAmount = 1f;
                        break;
                    default:
                        continue;
                }

                var newDamage = _ecsWorld.EnsureComponent<ReceivedDamageComponent>(pedEntity, out bool damageIsNew);
                float damage = damageMult * damageAmount;
                if (damageIsNew)
                {
                    newDamage.Damage = damage;
                }
                else
                {
                    newDamage.Damage += damage;
                }

                var newPain = _ecsWorld.EnsureComponent<ReceivedPainComponent>(pedEntity, out bool painIsNew);
                float pain = painMult * painAmount;
                if (painIsNew)
                {
                    newPain.Pain = pain;
                }
                else
                {
                    newPain.Pain += pain;
                }

                float bleeding = bleedingMult * bleedingAmount;
                var e = _ecsWorld.EnsureComponent<CreateBleedingEvent>(pedEntity, out _);
                e.BleedingToCreate.Enqueue(bleeding);
                
#if DEBUG
                _logger.MakeLog($"Ped {ped.Name(pedEntity)} got {wound} with " +
                                $"Damage {damage:0.0}; " +
                                $"Pain {pain:0.0}; " +
                                $"Bleeding {bleeding:0.0}");
#endif
            }
        }
    }
}
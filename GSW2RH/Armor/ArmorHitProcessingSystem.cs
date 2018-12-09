using System;
using System.Drawing;
using GunshotWound2.GswWorld;
using GunshotWound2.HitDetecting;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Armor
{
    [EcsInject]
    public class ArmorHitProcessingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent> _peds;
        private EcsFilter<GswPedComponent, DamagedBodyPartComponent, DamagedByWeaponComponent> _damagedPeds;

        private static readonly Random Random = new Random();
        private readonly GswLogger _logger;

        public ArmorHitProcessingSystem()
        {
            _logger = new GswLogger(typeof(ArmorHitProcessingSystem));
        }

        public void Run()
        {
            foreach (int i in _peds)
            {
                GswPedComponent gswPed = _peds.Components1[i];
                Ped ped = gswPed.ThisPed;
                if (!ped.Exists() || ped.Armor <= 0) continue;

                if (ped.Armor > gswPed.Armor)
                {
                    gswPed.Armor = ped.Armor;
                }
#if DEBUG
                if (gswPed.Armor <= 0) continue;

                Vector3 position = ped.AbovePosition;
                float maxArmor = ped.IsLocalPlayer
                    ? NativeFunction.Natives.GET_PLAYER_MAX_ARMOUR<int>(Game.LocalPlayer)
                    : 100;
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(1.05f, 0.15f, 0.1f), Color.LightSkyBlue);
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(gswPed.Armor / maxArmor, 0.1f, 0.1f),
                    Color.MediumBlue);
#endif
            }

            foreach (int i in _damagedPeds)
            {
                GswPedComponent gswPed = _damagedPeds.Components1[i];
                Ped ped = gswPed.ThisPed;
                if (!ped.Exists()) continue;

                if (gswPed.Armor <= 0)
                {
#if DEBUG
                    _logger.MakeLog("Ped " + ped.Name() + " doesn't have armor");
#endif
                    ped.Armor = gswPed.Armor;
                    continue;
                }

                BodyParts bodyPart = _damagedPeds.Components2[i].DamagedBodyPart;
                if (bodyPart != BodyParts.NECK && bodyPart != BodyParts.UPPER_BODY && bodyPart != BodyParts.LOWER_BODY)
                {
#if DEBUG
                    _logger.MakeLog(bodyPart + " of " + ped.Name() + " is not protected by armor");
#endif
                    ped.Armor = gswPed.Armor;
                    continue;
                }

                int weaponEntity = _damagedPeds.Components3[i].WeaponEntity;
                int pedEntity = _damagedPeds.Entities[i];
                var weaponStats = _ecsWorld.GetComponent<ArmorWeaponStatsComponent>(weaponEntity);
                if (weaponStats == null)
                {
#if DEBUG
                    _logger.MakeLog("This weapon doesn't have " + nameof(ArmorWeaponStatsComponent));
#endif
                    ped.Armor = gswPed.Armor;
                    continue;
                }

                gswPed.Armor -= weaponStats.ArmorDamage;
                if (gswPed.Armor <= 0)
                {
#if DEBUG
                    _logger.MakeLog("Armor of " + ped.Name() + " was destroyed");
#endif
                    ped.Armor = gswPed.Armor;
                    continue;
                }

                float maxArmor = ped.IsLocalPlayer
                    ? NativeFunction.Natives.GET_PLAYER_MAX_ARMOUR<int>(Game.LocalPlayer)
                    : 100;

                float armorPercent = gswPed.Armor / maxArmor;
                if (!weaponStats.CanPenetrateArmor || weaponStats.MinArmorPercentForPenetration < armorPercent)
                {
#if DEBUG
                    _logger.MakeLog("Armor of " + ped.Name() + " was not penetrated");
#endif
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    ped.Armor = gswPed.Armor;
                    continue;
                }

                float chanceToPenetrate = 1 - armorPercent / weaponStats.MinArmorPercentForPenetration;
                bool wasPenetrated = Random.IsTrueWithProbability(chanceToPenetrate);
                if (!wasPenetrated)
                {
#if DEBUG
                    _logger.MakeLog("Armor of " + ped.Name() + " was not penetrated, when chance was " +
                                    chanceToPenetrate);
#endif
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    ped.Armor = gswPed.Armor;
                    continue;
                }

#if DEBUG
                _logger.MakeLog("Armor of " + ped.Name() + " was penetrated, when chance was " + chanceToPenetrate);
#endif
                ped.Armor = gswPed.Armor;
            }
        }
    }
}
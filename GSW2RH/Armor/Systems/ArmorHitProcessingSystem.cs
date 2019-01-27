using System;
using System.Drawing;
using GunshotWound2.BodyParts;
using GunshotWound2.GswWorld;
using GunshotWound2.Pain;
using GunshotWound2.Utils;
using GunshotWound2.Weapons;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Armor.Systems
{
    [EcsInject]
    public class ArmorHitProcessingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, ArmorComponent> _pedsWithArmor;
        private EcsFilter<GswPedComponent, DamagedBodyPartComponent, DamagedByWeaponComponent, ArmorComponent> _damagedPedsWithArmor;

        private static readonly Random Random = new Random();
        private readonly GswLogger _logger;

        public ArmorHitProcessingSystem()
        {
            _logger = new GswLogger(typeof(ArmorHitProcessingSystem));
        }

        public void Run()
        {
            foreach (int i in _pedsWithArmor)
            {
                GswPedComponent gswPed = _pedsWithArmor.Components1[i];
                ArmorComponent armor = _pedsWithArmor.Components2[i];
                
                Ped ped = gswPed.ThisPed;
                if (!ped.Exists()) continue;

                if (ped.Armor > armor.Armor)
                {
                    armor.Armor = ped.Armor;
                }
#if DEBUG
                if (armor.Armor <= 0) continue;

                Vector3 position = ped.AbovePosition;
                float maxArmor = ped.IsLocalPlayer
                    ? NativeFunction.Natives.GET_PLAYER_MAX_ARMOUR<int>(Game.LocalPlayer)
                    : 100;
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(1.05f, 0.15f, 0.1f), Color.LightSkyBlue);
                Debug.DrawWireBoxDebug(position, ped.Orientation, new Vector3(armor.Armor / maxArmor, 0.1f, 0.1f), Color.MediumBlue);
#endif
            }

            foreach (int i in _damagedPedsWithArmor)
            {
                GswPedComponent gswPed = _damagedPedsWithArmor.Components1[i];
                ArmorComponent armor = _damagedPedsWithArmor.Components4[i];
                Ped ped = gswPed.ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = _damagedPedsWithArmor.Entities[i];
                if (armor.Armor <= 0)
                {
#if DEBUG
                    _logger.MakeLog($"Ped {ped.Name(pedEntity)} doesn't have armor");
#endif
                    ped.Armor = armor.Armor;
                    continue;
                }

                int bodyPartEntity = _damagedPedsWithArmor.Components2[i].DamagedBodyPartEntity;
                var bodyArmor = _ecsWorld.GetComponent<BodyPartArmorComponent>(bodyPartEntity);
                if (bodyArmor == null || !bodyArmor.ProtectedByBodyArmor)
                {
#if DEBUG
                    var partName = bodyPartEntity.GetEntityName(_ecsWorld);
                    _logger.MakeLog($"{partName} of {ped.Name(pedEntity)} is not protected by armor");
#endif
                    ped.Armor = armor.Armor;
                    continue;
                }

                int weaponEntity = _damagedPedsWithArmor.Components3[i].WeaponEntity;
                var weaponStats = _ecsWorld.GetComponent<ArmorWeaponStatsComponent>(weaponEntity);
                if (weaponStats == null)
                {
#if DEBUG
                    _logger.MakeLog($"This weapon doesn't have {nameof(ArmorWeaponStatsComponent)}");
#endif
                    ped.Armor = armor.Armor;
                    continue;
                }

                armor.Armor -= weaponStats.ArmorDamage;
                var newPain = _ecsWorld.EnsureComponent<AdditionalPainComponent>(pedEntity, out bool _);
                newPain.AdditionalPain = weaponStats.ArmorDamage;
#if DEBUG
                _logger.MakeLog($"Pain {newPain.AdditionalPain} by armor hit for ped {ped.Name(pedEntity)}");
#endif
                
                if (armor.Armor <= 0)
                {
#if DEBUG
                    _logger.MakeLog($"Armor of {ped.Name(pedEntity)} was destroyed");
#endif
                    ped.Armor = armor.Armor;
                    continue;
                }

                float maxArmor = ped.IsLocalPlayer
                    ? NativeFunction.Natives.GET_PLAYER_MAX_ARMOUR<int>(Game.LocalPlayer)
                    : 100;

                float armorPercent = armor.Armor / maxArmor;
                if (!weaponStats.CanPenetrateArmor || weaponStats.MinArmorPercentForPenetration < armorPercent)
                {
#if DEBUG
                    _logger.MakeLog($"Armor of {ped.Name(pedEntity)} was not penetrated");
#endif
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    _ecsWorld.RemoveComponent<DamagedBodyPartComponent>(pedEntity);
                    ped.Armor = armor.Armor;
                    continue;
                }

                float chanceToPenetrate = 1 - armorPercent / weaponStats.MinArmorPercentForPenetration;
                bool wasPenetrated = Random.IsTrueWithProbability(chanceToPenetrate);
                if (!wasPenetrated)
                {
#if DEBUG
                    _logger.MakeLog($"Armor of {ped.Name(pedEntity)} was not penetrated, when chance was {chanceToPenetrate}");
#endif
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    _ecsWorld.RemoveComponent<DamagedBodyPartComponent>(pedEntity);
                    ped.Armor = armor.Armor;
                    continue;
                }

#if DEBUG
                _logger.MakeLog($"Armor of {ped.Name(pedEntity)} was penetrated, when chance was {chanceToPenetrate}");
#endif
                ped.Armor = armor.Armor;
            }
        }
    }
}
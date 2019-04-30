using System;
using GSW3.Player;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.WoundEffects.WeaponDrop.Systems
{
    [EcsInject]
    public class WeaponDropSystem : BaseEffectSystem
    {
        private readonly EcsFilter<WeaponDropStatsComponent> _stats = null;

        public WeaponDropSystem() : base(new GswLogger(typeof(WeaponDropSystem)))
        {
        }

        protected override void ResetEffect(Ped ped, EcsEntity pedEntity)
        {
        }

        protected override void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity)
        {
            WeaponDropStatsComponent stats = _stats.Components1[0];
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            var component = EcsWorld.GetComponent<WeaponDropComponent>(woundEntity);
            if (component == null || ped.CurrentVehicle.Exists() || ped.Inventory.EquippedWeapon == null ||
                isPlayer && !stats.PlayerCanDropWeapon) return;

            ped.Inventory.EquippedWeapon.Drop();
#if DEBUG
            Logger.MakeLog($"{pedEntity.GetEntityName()} drop weapon");
#endif
            if(!ped.CombatTarget.Exists()) return;
            
            if (component.FleeIfHasNoWeapons && ped.Inventory.Weapons.Count <= 0)
            {
                ped.Tasks.ReactAndFlee(ped.CombatTarget);
#if DEBUG
                Logger.MakeLog($"{pedEntity.GetEntityName()} has no weapons and try to flee");
#endif
            }
            else if(component.TakeCoverDuration > 0)
            {
                ped.Tasks.TakeCoverFrom(ped.CombatTarget, component.TakeCoverDuration);
#if DEBUG
                Logger.MakeLog($"{pedEntity.GetEntityName()} try to take cover");
#endif
            }
        }
    }
}
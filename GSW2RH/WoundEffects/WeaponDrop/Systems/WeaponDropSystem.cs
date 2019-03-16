using System;
using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.WoundEffects.WeaponDrop.Systems
{
    [EcsInject]
    public class WeaponDropSystem : BaseEffectSystem
    {
        private readonly EcsFilter<WeaponDropStatsComponent> _stats = null;

        public WeaponDropSystem() : base(new GswLogger(typeof(WeaponDropSystem)))
        {
        }

        protected override void PreExecuteActions()
        {
            if (_stats.IsEmpty())
            {
                throw new Exception("WeaponDrop system was not init!");
            }
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            WeaponDropStatsComponent stats = _stats.Components1[0];
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (isPlayer && !stats.PlayerCanDropWeapon) return;

            ped.Inventory.EquippedWeapon?.Drop();
        }
    }
}
using System;
using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Effects.WeaponDrop.Systems
{
    [EcsInject]
    public class WeaponDropSystem : BaseEffectSystem
    {
        private EcsFilter<WeaponDropStatsComponent> _stats;
        
        public WeaponDropSystem() : base(new GswLogger(typeof(WeaponDropSystem)))
        {
        }

        protected override void PrepareRunActions()
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            if (_stats.EntitiesCount <= 0)
            {
                throw new Exception("WeaponDrop system was not init!");
            }

            WeaponDropStatsComponent stats = _stats.Components1[0];
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if(isPlayer && !stats.PlayerCanDropWeapon) return;
            
            ped.Inventory.EquippedWeapon?.Drop();
        }
    }
}
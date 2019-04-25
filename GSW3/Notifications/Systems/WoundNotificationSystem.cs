using System;
using System.Collections.Generic;
using GSW3.BodyParts;
using GSW3.Player;
using GSW3.Utils;
using GSW3.Weapons;
using GSW3.Wounds;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Notifications.Systems
{
    [EcsInject]
    public class WoundNotificationSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<
            DamagedByWeaponComponent,
            DamagedBodyPartComponent,
            WoundedComponent,
            PlayerMarkComponent> _woundedPlayer = null;
        
        public void Run()
        {
            if(_woundedPlayer.IsEmpty()) return;

            EcsEntity weaponEntity = _woundedPlayer.Components1[0].WeaponEntity;
            DamagedBodyPartComponent damagedBodyPart = _woundedPlayer.Components2[0];
            List<EcsEntity> woundList = _woundedPlayer.Components3[0].WoundEntities;
            
            int woundCount = 1;
            foreach (EcsEntity woundEntity in woundList)
            {
                var wounded = _ecsWorld.GetComponent<WoundComponent>(woundEntity);
                if(wounded == null) continue;
            
                string boneName = damagedBodyPart.DamagedBoneId.ToString();
                if (Enum.TryParse(boneName, out PedBoneId boneId))
                {
                    boneName = boneId.ToString();
                }

                _ecsWorld.CreateEntityWith(out NotificationComponent notification);
                if (woundCount++ <= 1)
                {
                    notification.Message = $"~s~Wound:~n~ ~o~{woundEntity.GetEntityName()} " +
                                           $"~s~on {damagedBodyPart.DamagedBodyPartEntity.GetEntityName()}" +
                                           $"~c~({boneName})~s~ " +
                                           $"by ~y~{weaponEntity.GetEntityName()}";
                }
                else
                {
                    notification.Message = $"~s~Crit:~n~ ~r~{woundEntity.GetEntityName()} " +
                                           $"~s~on {damagedBodyPart.DamagedBodyPartEntity.GetEntityName()}" +
                                           $"~c~({boneName})~s~ " +
                                           $"by ~y~{weaponEntity.GetEntityName()}";
                }
            }
        }
    }
}
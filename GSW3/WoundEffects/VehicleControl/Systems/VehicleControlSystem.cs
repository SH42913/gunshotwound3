using GSW3.Player;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.WoundEffects.VehicleControl.Systems
{
    [EcsInject]
    public class VehicleControlSystem : BaseEffectSystem
    {
        private readonly EcsFilter<PlayerMarkComponent, DisabledVehicleControlComponent> _disabledControls = null;
        
        public VehicleControlSystem() : base(new GswLogger(typeof(VehicleControlSystem)))
        {
        }

        protected override void PreExecuteActions()
        {
            if (!_disabledControls.IsEmpty())
            {
                DisableVehicleControl(true);
            }
        }

        protected override void ResetEffect(Ped ped, EcsEntity pedEntity)
        {
            var disabled = EcsWorld.GetComponent<DisabledVehicleControlComponent>(pedEntity);
            if (disabled == null) return;

            EcsWorld.RemoveComponent<DisabledVehicleControlComponent>(pedEntity);
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer)
            {
                ped.BlockPermanentEvents = false;
            }
        }

        protected override void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            var disabled = EcsWorld.GetComponent<DisabledVehicleControlComponent>(pedEntity);

            var disable = EcsWorld.GetComponent<DisableVehicleControlComponent>(woundEntity);
            if (disable != null && ped.CurrentVehicle.Exists())
            {
                if (disabled == null)
                {
                    EcsWorld
                        .AddComponent<DisabledVehicleControlComponent>(pedEntity)
                        .EnableOnlyOnHeal = disable.EnableOnlyOnHeal;

                    if (!isPlayer)
                    {
                        ped.Tasks.Clear();
                        ped.BlockPermanentEvents = true;
                        ped.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                    }
                }
                else
                {
                    disabled.EnableOnlyOnHeal |= disable.EnableOnlyOnHeal;
                }
            }

            var enable = EcsWorld.GetComponent<EnableVehicleControlComponent>(woundEntity);
            if (disabled == null || disabled.EnableOnlyOnHeal || enable == null) return;

            EcsWorld.RemoveComponent<DisabledVehicleControlComponent>(pedEntity);
            if (!isPlayer)
            {
                ped.BlockPermanentEvents = false;
            }
        }

        public static void DisableVehicleControl(bool disable)
        {
            Game.DisableControlAction(0, GameControl.VehicleAccelerate, disable);
            Game.DisableControlAction(0, GameControl.VehicleAim, disable);
            Game.DisableControlAction(0, GameControl.VehicleAttack, disable);
            Game.DisableControlAction(0, GameControl.VehicleAttack2, disable);
            Game.DisableControlAction(0, GameControl.VehicleBrake, disable);
            Game.DisableControlAction(0, GameControl.VehicleDuck, disable);
            Game.DisableControlAction(0, GameControl.VehicleExit, disable);
            Game.DisableControlAction(0, GameControl.VehicleHandbrake, disable);
            Game.DisableControlAction(0, GameControl.VehicleHeadlight, disable);
            Game.DisableControlAction(0, GameControl.VehicleHorn, disable);
            Game.DisableControlAction(0, GameControl.VehicleJump, disable);
            Game.DisableControlAction(0, GameControl.VehicleRoof, disable);
            Game.DisableControlAction(0, GameControl.VehicleShuffle, disable);
            Game.DisableControlAction(0, GameControl.VehicleSpecial, disable);
            Game.DisableControlAction(0, GameControl.VehicleDropProjectile, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyAttack, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyAttack2, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyDuck, disable);
            Game.DisableControlAction(0, GameControl.VehicleGrapplingHook, disable);
            Game.DisableControlAction(0, GameControl.VehicleGunDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleGunLeft, disable);
            Game.DisableControlAction(0, GameControl.VehicleGunRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleGunUp, disable);
            Game.DisableControlAction(0, GameControl.VehicleHotwireLeft, disable);
            Game.DisableControlAction(0, GameControl.VehicleHotwireRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveLeft, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveUp, disable);
            Game.DisableControlAction(0, GameControl.VehicleNextRadio, disable);
            Game.DisableControlAction(0, GameControl.VehiclePassengerAim, disable);
            Game.DisableControlAction(0, GameControl.VehiclePassengerAttack, disable);
            Game.DisableControlAction(0, GameControl.VehiclePrevRadio, disable);
            Game.DisableControlAction(0, GameControl.VehiclePushbikePedal, disable);
            Game.DisableControlAction(0, GameControl.VehiclePushbikeSprint, disable);
            Game.DisableControlAction(0, GameControl.VehicleRadioWheel, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubAscend, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubDescend, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyThrottleDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyThrottleUp, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyUnderCarriage, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyYawLeft, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyYawRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleGunLeftRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleGunUpDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveDownOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveLeftOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveLeftRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveRightOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveUpDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleMoveUpOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleNextRadioTrack, disable);
            Game.DisableControlAction(0, GameControl.VehiclePrevRadioTrack, disable);
            Game.DisableControlAction(0, GameControl.VehiclePushbikeFrontBrake, disable);
            Game.DisableControlAction(0, GameControl.VehiclePushbikeRearBrake, disable);
            Game.DisableControlAction(0, GameControl.VehicleSelectNextWeapon, disable);
            Game.DisableControlAction(0, GameControl.VehicleSelectPrevWeapon, disable);
            Game.DisableControlAction(0, GameControl.VehicleSpecialAbilityFranklin, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubThrottleDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubThrottleUp, disable);
            Game.DisableControlAction(0, GameControl.VehicleMouseControlOverride, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyMouseControlOverride, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyPitchDownOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyPitchUpDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyPitchUpOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyRollLeftOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyRollLeftRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyRollRightOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlySelectNextWeapon, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlySelectPrevWeapon, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlySelectTargetLeft, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlySelectTargetRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleFlyVerticalFlightMode, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubMouseControlOverride, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubPitchDownOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubPitchUpDown, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubPitchUpOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubTurnHardLeft, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubTurnHardRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubTurnLeftOnly, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubTurnLeftRight, disable);
            Game.DisableControlAction(0, GameControl.VehicleSubTurnRightOnly, disable);
        }
    }
}
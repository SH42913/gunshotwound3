using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.WoundEffects.CameraShake.Systems
{
    [EcsInject]
    public class CameraShakeSystem : BaseEffectSystem
    {
        private readonly EcsFilter<PermanentCameraShakeComponent> _permanentCameraShake = null;

        public CameraShakeSystem() : base(new GswLogger(typeof(CameraShakeSystem)))
        {
        }

        protected override void PrepareRunActions()
        {
            if (_permanentCameraShake.IsEmpty() || NativeFunction.Natives.IS_GAMEPLAY_CAM_SHAKING<bool>()) return;

            PermanentCameraShakeComponent shake = _permanentCameraShake.Components1[0];
            NativeFunction.Natives.SHAKE_GAMEPLAY_CAM(shake.ShakeName, shake.Intensity);
#if DEBUG
            Logger.MakeLog($"Start camera shake {shake.ShakeName} with {shake.Intensity}");
#endif
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer) return;

            NativeFunction.Natives.STOP_GAMEPLAY_CAM_SHAKING(true);
            EcsWorld.RemoveComponent<PermanentCameraShakeComponent>(pedEntity, true);
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if(!isPlayer) return;

            var pedPermanent = EcsWorld.GetComponent<PermanentCameraShakeComponent>(pedEntity);
            var permanent = EcsWorld.GetComponent<EnablePermanentCameraShakeComponent>(woundEntity);
            if (permanent != null)
            {
                if (pedPermanent != null && pedPermanent.Priority > permanent.Priority) return;
                
                var shake = EcsWorld.EnsureComponent<PermanentCameraShakeComponent>(pedEntity, out _);
                shake.DisableOnlyOnHeal = permanent.DisableOnlyOnHeal;
                shake.ShakeName = permanent.ShakeName;
                shake.Intensity = permanent.Intensity;
                shake.Priority = permanent.Priority;
                NativeFunction.Natives.STOP_GAMEPLAY_CAM_SHAKING(true);
                
#if DEBUG
                Logger.MakeLog($"Enable permanent camera shake, only on heal - {permanent.DisableOnlyOnHeal}");
#endif

                return;
            }

            if (pedPermanent == null || pedPermanent.DisableOnlyOnHeal) return;

            var disable = EcsWorld.GetComponent<DisableCameraShakeComponent>(woundEntity);
            if (disable == null) return;

            NativeFunction.Natives.STOP_GAMEPLAY_CAM_SHAKING(true);
            EcsWorld.RemoveComponent<PermanentCameraShakeComponent>(pedEntity);
#if DEBUG
            Logger.MakeLog("Disable camera shake");
#endif
        }
    }
}
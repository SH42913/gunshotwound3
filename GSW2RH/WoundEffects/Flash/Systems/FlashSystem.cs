using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.WoundEffects.Flash.Systems
{
    [EcsInject]
    public class FlashSystem : BaseEffectSystem
    {
        public FlashSystem() : base(new GswLogger(typeof(FlashSystem)))
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer) return;

            var createFlash = EcsWorld.GetComponent<CreateFlashComponent>(woundEntity);
            if (createFlash == null) return;

            NativeFunction.Natives.SET_FLASH(1, 1, createFlash.FadeIn, createFlash.Duration, createFlash.FadeOut);
#if DEBUG
            Logger.MakeLog($"Create flash with {createFlash.Duration} duration");
#endif
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
        }
    }
}
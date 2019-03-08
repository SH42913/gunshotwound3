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

        protected override void PrepareRunActions()
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer) return;

            bool createFlash = EcsWorld.GetComponent<CreateFlashComponent>(woundEntity) != null;
            if (!createFlash) return;

            NativeFunction.Natives.SET_FLASH(1, 1, 250, 500, 250);
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
        }
    }
}
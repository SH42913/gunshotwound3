using GSW3.GswWorld;
using GSW3.Pain;
using GSW3.Player;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GSW3.WoundEffects.MovementClipset.Systems
{
    [EcsInject]
    public class MovementClipsetSystem : BaseEffectSystem
    {
        private readonly EcsFilter<GswPedComponent, PainIsGoneComponent> _pedsWithoutPain = null;

        public MovementClipsetSystem() : base(new GswLogger(typeof(MovementClipsetSystem)))
        {
        }

        protected override void PreExecuteActions()
        {
            foreach (int i in _pedsWithoutPain)
            {
                Ped ped = _pedsWithoutPain.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                NativeFunction.Natives.RESET_PED_MOVEMENT_CLIPSET(ped, 0.0f);
            }
        }

        protected override void ResetEffect(Ped ped, EcsEntity pedEntity)
        {
            NativeFunction.Natives.RESET_PED_MOVEMENT_CLIPSET(ped, 0.0f);
        }

        protected override void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity)
        {
            var clipset = EcsWorld.GetComponent<NewMovementClipsetComponent>(woundEntity);
            if (clipset == null) return;

            var player = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity);
            if (player != null)
            {
                ApplyClipset(ped, clipset.Player, pedEntity);
            }
            else if (ped.IsMale)
            {
                ApplyClipset(ped, clipset.PedMale, pedEntity);
            }
            else
            {
                ApplyClipset(ped, clipset.PedFemale, pedEntity);
            }
        }

        private void ApplyClipset(Ped ped, string clipset, EcsEntity pedEntity)
        {
            if (string.IsNullOrEmpty(clipset)) return;

            if (!NativeFunction.Natives.HAS_ANIM_SET_LOADED<bool>(clipset))
            {
                NativeFunction.Natives.REQUEST_ANIM_SET(clipset);
            }

            NativeFunction.Natives.SET_PED_MOVEMENT_CLIPSET(ped, clipset, 1.0f);
#if DEBUG
            Logger.MakeLog($"{pedEntity.GetEntityName()} changed movement clipset to {clipset}");
#endif
        }
    }
}
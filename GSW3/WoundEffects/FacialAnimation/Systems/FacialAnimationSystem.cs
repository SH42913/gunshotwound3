using System;
using GSW3.GswWorld;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GSW3.WoundEffects.FacialAnimation.Systems
{
    [EcsInject]
    public class FacialAnimationSystem : BaseEffectSystem
    {
        private readonly EcsFilter<GswPedComponent, PermanentFacialAnimationComponent> _permanentAnimPeds = null;
        private readonly Random _random = null;

        public FacialAnimationSystem() : base(new GswLogger(typeof(FacialAnimationSystem)))
        {
        }

        protected override void PreExecuteActions()
        {
            foreach (int i in _permanentAnimPeds)
            {
                Ped ped = _permanentAnimPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                PermanentFacialAnimationComponent animation = _permanentAnimPeds.Components2[i];
                NativeFunction.Natives.PLAY_FACIAL_ANIM(ped, animation.Name, animation.Dict);
            }
        }

        protected override void ResetEffect(Ped ped, EcsEntity pedEntity)
        {
            var permanent = EcsWorld.GetComponent<PermanentFacialAnimationComponent>(pedEntity);
            if (permanent == null) return;

            EcsWorld.RemoveComponent<PermanentFacialAnimationComponent>(pedEntity);
        }

        protected override void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity)
        {
            var enable = EcsWorld.GetComponent<EnableFacialAnimationComponent>(woundEntity);
            if (enable != null)
            {
                string animationName = _random.NextFromList(enable.Animations);
                string dict = ped.IsMale
                    ? enable.MaleDict
                    : enable.FemaleDict;

                if (enable.Permanent)
                {
                    var permanent = EcsWorld.EnsureComponent<PermanentFacialAnimationComponent>(pedEntity, out _);
                    permanent.Name = animationName;
                    permanent.Dict = dict;
#if DEBUG
                    Logger.MakeLog($"{pedEntity.GetEntityName()} got permanent facial animation {animationName}");
#endif
                }
                else
                {
                    NativeFunction.Natives.PLAY_FACIAL_ANIM(ped, animationName, dict);
#if DEBUG
                    Logger.MakeLog($"{pedEntity.GetEntityName()} got short facial animation {animationName}");
#endif
                }
            }

            var disable = EcsWorld.GetComponent<DisableFacialAnimationComponent>(woundEntity);
            if (disable != null)
            {
                EcsWorld.RemoveComponent<PermanentFacialAnimationComponent>(pedEntity, true);
#if DEBUG
                Logger.MakeLog($"{pedEntity.GetEntityName()} clean facial animation");
#endif
            }
        }
    }
}
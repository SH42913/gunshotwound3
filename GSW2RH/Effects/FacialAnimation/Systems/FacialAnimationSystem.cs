using System;
using GunshotWound2.GswWorld;
using GunshotWound2.Health;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Effects.FacialAnimation.Systems
{
    [EcsInject]
    public class FacialAnimationSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, WoundedComponent> _woundedPeds;
        private EcsFilter<GswPedComponent, PermanentFacialAnimationComponent> _permanentAnimPeds;
        private EcsFilter<GswPedComponent, PermanentFacialAnimationComponent, FullyHealedComponent> _healedPeds;

        private readonly GswLogger _logger;
        private static readonly Random Random = new Random();
        
        public FacialAnimationSystem()
        {
            _logger = new GswLogger(typeof(FacialAnimationSystem));
        }
        
        public void Run()
        {
            foreach (int pedIndex in _woundedPeds)
            {
                Ped ped = _woundedPeds.Components1[pedIndex].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = _woundedPeds.Entities[pedIndex];
                WoundedComponent wounded = _woundedPeds.Components2[pedIndex];

                foreach (int woundEntity in wounded.WoundEntities)
                {
                    var enable = _ecsWorld.GetComponent<EnableFacialAnimationComponent>(woundEntity);
                    if (enable != null)
                    {
                        string animationName = Random.NextFromList(enable.Animations);
                        string dict = ped.IsMale 
                            ? enable.MaleDict 
                            : enable.FemaleDict;
                        
                        if (enable.Permanent)
                        {
                            var permanent = _ecsWorld.EnsureComponent<PermanentFacialAnimationComponent>(pedEntity, out _);
                            permanent.Name = animationName;
                            permanent.Dict = dict;
#if DEBUG
                            _logger.MakeLog($"{ped.Name(pedEntity)} got permanent facial animation {animationName}");
#endif
                        }
                        else
                        {
                            NativeFunction.Natives.PLAY_FACIAL_ANIM(ped, animationName, dict);
#if DEBUG
                            _logger.MakeLog($"{ped.Name(pedEntity)} got short facial animation {animationName}");
#endif
                        }
                    }

                    var disable = _ecsWorld.GetComponent<DisableFacialAnimationComponent>(woundEntity);
                    if (disable != null)
                    {
                        _ecsWorld.RemoveComponent<PermanentFacialAnimationComponent>(pedEntity, true);
                    }
                }
            }

            foreach (int i in _permanentAnimPeds)
            {
                Ped ped = _permanentAnimPeds.Components1[i].ThisPed;
                if(!ped.Exists()) continue;

                PermanentFacialAnimationComponent animation = _permanentAnimPeds.Components2[i];
                NativeFunction.Natives.PLAY_FACIAL_ANIM(ped, animation.Name, animation.Dict);
            }

            foreach (int i in _healedPeds)
            {
                Ped ped = _healedPeds.Components1[i].ThisPed;
                if(!ped.Exists()) continue;

                int pedEntity = _healedPeds.Entities[i];
                _ecsWorld.RemoveComponent<PermanentFacialAnimationComponent>(pedEntity, true);
            }
        }
    }
}
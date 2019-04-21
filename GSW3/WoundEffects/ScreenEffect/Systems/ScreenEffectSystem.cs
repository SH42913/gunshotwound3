using GSW3.Player;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GSW3.WoundEffects.ScreenEffect.Systems
{
    [EcsInject]
    public class ScreenEffectSystem : BaseEffectSystem
    {
        public ScreenEffectSystem() : base(new GswLogger(typeof(ScreenEffectSystem)))
        {
        }

        protected override void ResetEffect(Ped ped, EcsEntity pedEntity)
        {   
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer) return;
            
            NativeFunction.Natives.xB4EDDC19532BFB85();
            EcsWorld.RemoveComponent<MainScreenEffectComponent>(pedEntity, true);
        }

        protected override void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity)
        {
            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            if (!isPlayer) return;

            var mainEffect = EcsWorld.GetComponent<MainScreenEffectComponent>(pedEntity);
            var start = EcsWorld.GetComponent<StartScreenEffectComponent>(woundEntity);
            if (start != null)
            {
                if (mainEffect != null && (!start.Main || mainEffect.Name.Equals(start.Name))) return;
                
                NativeFunction.Natives.x2206BF9A37B7F724(start.Name, start.Duration, start.Loop);
                EcsWorld.EnsureComponent<MainScreenEffectComponent>(pedEntity, out _).Name = start.Name;
#if DEBUG
                Logger.MakeLog($"Start Effect {start.Name}, duration {start.Duration}, loop {start.Loop}");
#endif
            }

            var stop = EcsWorld.GetComponent<StopScreenEffectComponent>(woundEntity);
            if (stop != null)
            {
                NativeFunction.Natives.x068E835A1D0DC0E3(stop.Name);
                if (mainEffect != null && mainEffect.Name.Equals(stop.Name))
                {
                    EcsWorld.RemoveComponent<MainScreenEffectComponent>(pedEntity);
                }
                
#if DEBUG
                Logger.MakeLog($"Stop Effect {stop.Name}");
#endif
            }
        }
    }
}
using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.WoundEffects.PainSound.Systems
{
    [EcsInject]
    public class PlayPainSystem : BaseEffectSystem
    {
        public PlayPainSystem() : base(new GswLogger(typeof(PlayPainSystem)))
        {
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            var play = EcsWorld.GetComponent<PlayPainComponent>(woundEntity);
            if (play == null) return;

            bool isPlayer = EcsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
            bool isMale = ped.IsMale;

            if (isPlayer)
            {
                if(play.Player < 0) return;
                NativeFunction.Natives.PLAY_PAIN(ped, play.Player, 0, 0);
#if DEBUG
                Logger.MakeLog($"Play pain {play.Player} for player");
#endif
            }
            else if (isMale)
            {
                if(play.Male < 0) return;
                NativeFunction.Natives.PLAY_PAIN(ped, play.Male, 0, 0);
#if DEBUG
                Logger.MakeLog($"Play pain {play.Male} for male");
#endif
            }
            else
            {
                if(play.Female < 0) return;
                NativeFunction.Natives.PLAY_PAIN(ped, play.Female, 0, 0);
#if DEBUG
                Logger.MakeLog($"Play pain {play.Female} for female");
#endif
            }
        }
    }
}
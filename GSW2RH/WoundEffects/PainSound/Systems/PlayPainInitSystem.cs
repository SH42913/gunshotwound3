using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.PainSound.Systems
{
    [EcsInject]
    public class PlayPainInitSystem : BaseEffectInitSystem
    {
        public PlayPainInitSystem() : base(new GswLogger(typeof(PlayPainInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement play = partRoot.Element("PlayPainSound");
            if(play == null) return;

            var component = EcsWorld.AddComponent<PlayPainComponent>(partEntity);
            component.Player = play.GetInt("Player");
            component.Male = play.GetInt("Male");
            component.Female = play.GetInt("Female");
        }
    }
}
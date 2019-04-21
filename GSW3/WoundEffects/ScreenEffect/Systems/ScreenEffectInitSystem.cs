using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.ScreenEffect.Systems
{
    [EcsInject]
    public class ScreenEffectInitSystem : BaseEffectInitSystem
    {
        public ScreenEffectInitSystem() : base(new GswLogger(typeof(ScreenEffectInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement startEffect = partRoot.Element("StartScreenEffect");
            if (startEffect != null)
            {
                var effect = EcsWorld.AddComponent<StartScreenEffectComponent>(partEntity);
                effect.Name = startEffect.GetAttributeValue("Name");
                effect.Duration = startEffect.GetInt("Duration");
                effect.Loop = startEffect.GetBool("Loop");
                effect.Main = startEffect.GetBool("Main");
            }

            XElement stopEffect = partRoot.Element("StopScreenEffect");
            if (stopEffect != null)
            {
                var effect = EcsWorld.AddComponent<StopScreenEffectComponent>(partEntity);
                effect.Name = stopEffect.GetAttributeValue("Name");
            }
        }
    }
}
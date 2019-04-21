using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.Flash.Systems
{
    [EcsInject]
    public class FlashInitSystem : BaseEffectInitSystem
    {
        public FlashInitSystem() : base(new GswLogger(typeof(FlashInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement flash = partRoot.Element("CreateFlash");
            if(flash == null) return;

            var component = EcsWorld.AddComponent<CreateFlashComponent>(partEntity);
            component.FadeIn = flash.GetFloat("FadeIn");
            component.FadeOut = flash.GetFloat("FadeOut");
            component.Duration = component.FadeIn + component.FadeOut;
        }
    }
}
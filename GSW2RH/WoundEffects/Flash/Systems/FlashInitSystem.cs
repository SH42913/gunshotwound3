using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.Flash.Systems
{
    [EcsInject]
    public class FlashInitSystem : BaseEffectInitSystem
    {
        public FlashInitSystem() : base(new GswLogger(typeof(FlashInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
        {
            XElement flash = partRoot.Element("CreateFlash");
            if(flash == null) return;

            EcsWorld.AddComponent<CreateFlashComponent>(partEntity);
        }
    }
}
using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.MovementClipset.Systems
{
    [EcsInject]
    public class MovementClipsetInitSystem : BaseEffectInitSystem
    {
        public MovementClipsetInitSystem() : base(new GswLogger(typeof(MovementClipsetInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
        {
            var clipset = partRoot.Element("MovementClipset");
            if (clipset == null) return;

            var component = EcsWorld.AddComponent<NewMovementClipsetComponent>(partEntity);
            component.Player = clipset.GetAttributeValue("Player");
            component.PedMale = clipset.GetAttributeValue("PedMale");
            component.PedFemale = clipset.GetAttributeValue("PedFemale");

#if DEBUG
            Logger.MakeLog($"{partEntity.GetEntityName(EcsWorld)} got {component}");
#endif
        }
    }
}
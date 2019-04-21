using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.InstantKill.Systems
{
    public class InstantKillInitSystem : BaseEffectInitSystem
    {
        public InstantKillInitSystem() : base(new GswLogger(typeof(InstantKillInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement element = partRoot.Element("InstantKill");
            if (element == null) return;

            EcsWorld
                .AddComponent<InstantKillComponent>(partEntity)
                .ReasonKey = element.GetAttributeValue("ReasonKey");

#if DEBUG
            Logger.MakeLog($"{partEntity.GetEntityName()} will instant kill");
#endif
        }
    }
}
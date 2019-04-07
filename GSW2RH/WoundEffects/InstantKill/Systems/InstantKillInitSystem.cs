using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.InstantKill.Systems
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
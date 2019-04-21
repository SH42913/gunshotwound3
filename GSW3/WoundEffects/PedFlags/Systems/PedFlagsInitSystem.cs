using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.PedFlags.Systems
{
    [EcsInject]
    public class PedFlagsInitSystem : BaseEffectInitSystem
    {
        public PedFlagsInitSystem() : base(new GswLogger(typeof(PedFlagsInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement change = partRoot.Element("ChangePedFlag");
            if(change == null) return;

            var component = EcsWorld.AddComponent<ChangePedFlagComponent>(partEntity);
            component.Id = change.GetInt("Id");
            component.Value = change.GetBool();
            component.ForPlayer = change.GetBool("ForPlayer");
        }
    }
}
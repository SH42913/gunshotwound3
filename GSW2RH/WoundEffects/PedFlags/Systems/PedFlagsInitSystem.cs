using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.PedFlags.Systems
{
    [EcsInject]
    public class PedFlagsInitSystem : BaseEffectInitSystem
    {
        public PedFlagsInitSystem() : base(new GswLogger(typeof(PedFlagsInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
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
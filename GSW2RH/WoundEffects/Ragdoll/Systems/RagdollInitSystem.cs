using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.Ragdoll.Systems
{
    [EcsInject]
    public class RagdollInitSystem : BaseEffectInitSystem
    {
        public RagdollInitSystem() : base(new GswLogger(typeof(RagdollInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
        {
            XElement ragdoll = partRoot.Element("EnableRagdoll");
            if (ragdoll != null)
            {
                var component = EcsWorld.AddComponent<EnableRagdollComponent>(partEntity);
                component.LengthInMs = ragdoll.GetInt("LengthInMs");
                component.Type = ragdoll.GetInt("Type");
                component.Permanent = ragdoll.GetBool("Permanent");
                component.DisableOnlyOnHeal = ragdoll.GetBool("DisableOnlyOnHeal");

#if DEBUG
                Logger.MakeLog($"{partEntity.GetEntityName()} have got {component}");
#endif
            }

            XElement disable = partRoot.Element("DisablePermanentRagdoll");
            if (disable != null)
            {
                EcsWorld.AddComponent<DisablePermanentRagdollComponent>(partEntity);

#if DEBUG
                Logger.MakeLog($"{partEntity.GetEntityName()} will disable permanent ragdoll");
#endif
            }
        }
    }
}
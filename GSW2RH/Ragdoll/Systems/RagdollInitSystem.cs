using System.Xml.Linq;
using GunshotWound2.Bleeding;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Ragdoll.Systems
{
    [EcsInject]
    public class RagdollInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<LoadedItemConfigComponent> _initParts;

        private readonly GswLogger _logger;

        public RagdollInitSystem()
        {
            _logger = new GswLogger(typeof(RagdollInitSystem));
        }

        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement partRoot = _initParts.Components1[i].ElementRoot;
                int partEntity = _initParts.Entities[i];

                XElement ragdoll = partRoot.Element("EnableRagdoll");
                if (ragdoll != null)
                {
                    var component = _ecsWorld.AddComponent<EnableRagdollComponent>(partEntity);
                    component.LengthInMs = ragdoll.GetInt("LengthInMs");
                    component.Type = ragdoll.GetInt("Type");
                    component.Permanent = ragdoll.GetBool("Permanent");
                    component.DisableOnlyOnHeal = ragdoll.GetBool("DisableOnlyOnHeal");

#if DEBUG
                    _logger.MakeLog($"{partEntity.GetEntityName(_ecsWorld)} got {component}");
#endif
                }

                XElement disable = partRoot.Element("DisablePermanentRagdoll");
                if (disable != null)
                {
                    _ecsWorld.AddComponent<DisablePermanentRagdollComponent>(partEntity);
                }
            }
        }

        public void Destroy()
        {
        }
    }
}
using System.Xml.Linq;
using GunshotWound2.Bodies;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Armor
{
    [EcsInject]
    public class BodyPartArmorInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<BodyPartComponent, InitElementComponent, HashesComponent> _initParts;

        private GswLogger _logger;

        public BodyPartArmorInitSystem()
        {
            _logger = new GswLogger(typeof(BodyPartInitSystem));
        }

        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                int entity = _initParts.Entities[i];
                XElement partRoot = _initParts.Components2[i].ElementRoot;
                XElement protection = partRoot.Element("Protection");

                var bodyArmor = _ecsWorld.AddComponent<BodyPartArmorComponent>(entity);
                bodyArmor.ProtectedByHelmet = protection.GetBool("ByHelmet");
                bodyArmor.ProtectedByBodyArmor = protection.GetBool("ByArmor");

#if DEBUG
                string partName = _initParts.Components3[i].Name;
                _logger.MakeLog($"BodyPart {partName} got {bodyArmor}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
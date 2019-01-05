using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Pain
{
    [EcsInject]
    public class PainMultInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<InitElementComponent, HashesComponent> _initParts;

        private readonly GswLogger _logger;
        
        public PainMultInitSystem()
        {
            _logger = new GswLogger(typeof(PainMultInitSystem));
        }
        
        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement weaponRoot = _initParts.Components1[i].ElementRoot;
                XElement multElement = weaponRoot.GetElement("PainMult");
                int weaponEntity = _initParts.Entities[i];

                var mult = _ecsWorld.AddComponent<PainMultComponent>(weaponEntity);
                mult.Multiplier = multElement.GetFloat();

#if DEBUG
                string name = _initParts.Components2[i].Name;
                _logger.MakeLog($"{name} got {mult}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
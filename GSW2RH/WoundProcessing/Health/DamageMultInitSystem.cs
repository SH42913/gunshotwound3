using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Health
{
    [EcsInject]
    public class DamageMultInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<InitElementComponent, HashesComponent> _initParts;

        private readonly GswLogger _logger;
        
        public DamageMultInitSystem()
        {
            _logger = new GswLogger(typeof(DamageMultInitSystem));
        }
        
        public void Initialize()
        {
            foreach (int i in _initParts)
            {
                XElement weaponRoot = _initParts.Components1[i].ElementRoot;
                XElement multElement = weaponRoot.GetElement("DamageMult");
                int weaponEntity = _initParts.Entities[i];

                var damageMult = _ecsWorld.AddComponent<DamageMultComponent>(weaponEntity);
                damageMult.Multiplier = multElement.GetFloat();

#if DEBUG
                string name = _initParts.Components2[i].Name;
                _logger.MakeLog($"{name} got {damageMult}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
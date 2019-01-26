using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Wounds.Systems
{
    [EcsInject]
    public class WoundSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<WoundedComponent> _woundedPeds;

        private readonly GswLogger _logger;

        public WoundSystem()
        {
            _logger = new GswLogger(typeof(WoundSystem));
        }

        public void Run()
        {
#if DEBUG
            foreach (int i in _woundedPeds)
            {
                WoundedComponent wounded = _woundedPeds.Components1[i];
                int woundedEntity = _woundedPeds.Entities[i];

                string woundList = "Wounds: ";
                foreach (int woundEntity in wounded.WoundEntities)
                {
                    woundList += $"{woundEntity.GetEntityName(_ecsWorld)}, ";
                }

                _logger.MakeLog($"Entity {woundedEntity} {woundList}");
            }
#endif
        }
    }
}
using GunshotWound2.Health;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Bleeding.Systems
{
    [EcsInject]
    public class BleedingHealSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<BleedingInfoComponent, FullyHealedComponent> _healedEntities = null;

        private readonly GswLogger _logger;

        public BleedingHealSystem()
        {
            _logger = new GswLogger(typeof(BleedingHealSystem));
        }

        public void Run()
        {
            foreach (int i in _healedEntities)
            {
                BleedingInfoComponent info = _healedEntities.Components1[i];

                foreach (int bleedingEntity in info.BleedingEntities)
                {
                    if (!_ecsWorld.IsEntityExists(bleedingEntity)) continue;

                    _ecsWorld.RemoveEntity(bleedingEntity);
                }
            }
        }
    }
}
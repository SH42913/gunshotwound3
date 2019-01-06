using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Bleeding.Systems
{
    [EcsInject]
    public class BleedingCleanSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<BleedingInfoComponent, RemovedPedMarkComponent> _pedsToRemove;

        private readonly GswLogger _logger;

        public BleedingCleanSystem()
        {
            _logger = new GswLogger(typeof(BleedingCleanSystem));
        }
        
        public void Run()
        {
            foreach (int i in _pedsToRemove)
            {
                BleedingInfoComponent info = _pedsToRemove.Components1[i];
                int pedEntity = _pedsToRemove.Entities[i];
                
                foreach (int entity in info.BleedingEntities)
                {
                    _ecsWorld.RemoveEntity(entity);
                }
                
                _ecsWorld.RemoveComponent<BleedingInfoComponent>(pedEntity);
            }
        }
    }
}
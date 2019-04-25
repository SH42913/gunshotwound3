using GSW3.GswWorld;
using GSW3.Health;
using Leopotam.Ecs;

namespace GSW3.Bleeding.Systems
{
    [EcsInject]
    public class BleedingCleanSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        
        private readonly EcsFilter<PedBleedingInfoComponent, RemovedPedMarkComponent> _pedsToRemove = null;
        private readonly EcsFilter<PedBleedingInfoComponent, FullyHealedComponent> _healedEntities = null;
        
        public void Run()
        {
            foreach (int i in _pedsToRemove)
            {
                PedBleedingInfoComponent info = _pedsToRemove.Components1[i];
                EcsEntity pedEntity = _pedsToRemove.Entities[i];
                foreach (EcsEntity entity in info.BleedingEntities)
                {
                    _ecsWorld.RemoveEntity(entity);
                }
                
                _ecsWorld.RemoveComponent<PedBleedingInfoComponent>(pedEntity);
            }
            
            _ecsWorld.ProcessDelayedUpdates();
            
            foreach (int i in _healedEntities)
            {
                PedBleedingInfoComponent info = _healedEntities.Components1[i];
                foreach (EcsEntity bleedingEntity in info.BleedingEntities)
                {
                    if (!_ecsWorld.IsEntityExists(bleedingEntity)) continue;

                    _ecsWorld.RemoveEntity(bleedingEntity);
                }
            }
        }
    }
}
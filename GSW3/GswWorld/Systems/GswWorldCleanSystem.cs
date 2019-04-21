using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.GswWorld.Systems
{
    [EcsInject]
    public class GswWorldCleanSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<GswWorldComponent> _world = null;
        private readonly EcsFilter<GswPedComponent, RemovedPedMarkComponent> _pedsToRemove = null;

#if DEBUG
        private readonly GswLogger _logger;

        public GswWorldCleanSystem()
        {
            _logger = new GswLogger(typeof(GswWorldCleanSystem));
        }
#endif

        public void Run()
        {
            GswWorldComponent gswWorld = _world.Components1[0];
            foreach (int i in _pedsToRemove)
            {
                Ped ped = _pedsToRemove.Components1[i].ThisPed;
                EcsEntity pedEntity = _pedsToRemove.Entities[i];
                
                gswWorld.PedsToEntityDict.Remove(ped);
                _ecsWorld.RemoveEntity(pedEntity);

#if DEBUG
                _logger.MakeLog($"{pedEntity.GetEntityName()} was removed");
#endif
            }
        }
    }
}
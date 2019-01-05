using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld
{
    [EcsInject]
    public class GswWorldCleanSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<GswWorldComponent> _world;
        private EcsFilter<GswPedComponent, RemovedPedMarkComponent> _pedsToRemove;

        public void Run()
        {
            if (_world.EntitiesCount <= 0) return;
            GswWorldComponent gswWorld = _world.Components1[0];

            foreach (int i in _pedsToRemove)
            {
                Ped ped = _pedsToRemove.Components1[i].ThisPed;
                int pedEntity = _pedsToRemove.Entities[i];

                gswWorld.PedsToEntityDict.Remove(ped);
                _ecsWorld.RemoveEntity(pedEntity);
            }
        }
    }
}
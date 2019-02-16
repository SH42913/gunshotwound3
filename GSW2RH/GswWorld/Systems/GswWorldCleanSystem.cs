using System;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld.Systems
{
    [EcsInject]
    public class GswWorldCleanSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<GswWorldComponent> _world = null;
        private readonly EcsFilter<GswPedComponent, RemovedPedMarkComponent> _pedsToRemove = null;

        public void Run()
        {
            if (_world.IsEmpty())
            {
                throw new Exception("GswWorld was not init!");
            }
            
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
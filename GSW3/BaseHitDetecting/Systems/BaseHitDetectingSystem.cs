using GSW3.GswWorld;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GSW3.BaseHitDetecting.Systems
{
    [EcsInject]
    public class BaseHitDetectingSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly GameService _gameService = null;
        private readonly EcsFilter<GswPedComponent> _peds = null;

#if DEBUG
        private readonly GswLogger _logger;

        public BaseHitDetectingSystem()
        {
            _logger = new GswLogger(typeof(BaseHitDetectingSystem));
        }
#endif

        public void Run()
        {
            if(_gameService.GameIsPaused) return;
            
            foreach (int i in _peds)
            {
                Ped ped = _peds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                bool damaged = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_PED<bool>(ped);
                if (!damaged) continue;

                EcsEntity pedEntity = _peds.Entities[i];
                _ecsWorld.AddComponent<HasBeenHitMarkComponent>(pedEntity);
#if DEBUG
                _logger.MakeLog($"{pedEntity.GetEntityName()} has been damaged");
#endif
            }
        }
    }
}
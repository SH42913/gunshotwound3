using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.BaseHitDetecting.Systems
{
    [EcsInject]
    public class BaseHitDetectingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent> _peds;

        private readonly GswLogger _logger;

        public BaseHitDetectingSystem()
        {
            _logger = new GswLogger(typeof(BaseHitDetectingSystem));
        }

        public void Run()
        {
            foreach (int i in _peds)
            {
                Ped ped = _peds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                bool damaged = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_PED<bool>(ped);
                if (!damaged) continue;

                int pedEntity = _peds.Entities[i];
#if DEBUG
                _logger.MakeLog($"Ped {ped.Name(pedEntity)} has been damaged");
#endif
                _ecsWorld.AddComponent<HasBeenHitMarkComponent>(pedEntity);
            }
        }
    }
}
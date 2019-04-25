using GSW3.GswWorld;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Health.Systems
{
    [EcsInject]
    public class FullHealDetectSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<GswPedComponent, HealthComponent> _pedsToCheck = null;

#if DEBUG
        private readonly GswLogger _logger;

        public FullHealDetectSystem()
        {
            _logger = new GswLogger(typeof(FullHealDetectSystem));
        }
#endif

        public void Run()
        {
            foreach (int i in _pedsToCheck)
            {
                Ped ped = _pedsToCheck.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                HealthComponent health = _pedsToCheck.Components2[i];
                float realHealth = ped.GetHealth();
                if (realHealth <= health.MaxHealth) continue;

                EcsEntity entity = _pedsToCheck.Entities[i];
                _ecsWorld.AddComponent<FullyHealedComponent>(entity);
#if DEBUG
                _logger.MakeLog($"{entity.GetEntityName()} was fully healed!");
#endif
            }
        }
    }
}
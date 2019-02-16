using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Health.Systems
{
    [EcsInject]
    public class HealDetectSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<GswPedComponent, HealthComponent> _pedsToCheck = null;

        private readonly GswLogger _logger;

        public HealDetectSystem()
        {
            _logger = new GswLogger(typeof(HealDetectSystem));
        }

        public void Run()
        {
            foreach (int i in _pedsToCheck)
            {
                Ped ped = _pedsToCheck.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                HealthComponent health = _pedsToCheck.Components2[i];
                float realHealth = ped.GetHealth();
                if (realHealth <= health.MaxHealth) continue;

                int entity = _pedsToCheck.Entities[i];
                _ecsWorld.AddComponent<FullyHealedComponent>(entity);
#if DEBUG
                _logger.MakeLog($"{entity.GetEntityName()} was fully healed!");
#endif
            }
        }
    }
}
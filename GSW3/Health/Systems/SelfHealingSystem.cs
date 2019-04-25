using GSW3.Bleeding;
using GSW3.GswWorld;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Health.Systems
{
    [EcsInject]
    public class SelfHealingSystem : IEcsRunSystem
    {
        private readonly GameService _gameService = null;

        private readonly EcsFilter<HealthStatsComponent> _healthStats = null;
        private readonly EcsFilter<GswPedComponent, HealthComponent, PedBleedingInfoComponent> _pedsWithHealth = null;

        public void Run()
        {
            if (_gameService.GameIsPaused) return;

            float dt = GswExtensions.GetDeltaTime();
            float rate = _healthStats.Components1[0].SelfHealingRate;
            if(dt <= 0 || rate <= 0) return;

            foreach (int i in _pedsWithHealth)
            {
                Ped ped = _pedsWithHealth.Components1[i].ThisPed;
                if(!ped.Exists()) continue;
                
                PedBleedingInfoComponent info = _pedsWithHealth.Components3[i];
                if (info.BleedingEntities.Count > 0) continue;

                HealthComponent health = _pedsWithHealth.Components2[i];
                if (health.MaxHealth - health.Health < 5f) continue;

                health.Health += rate * dt;
                ped.SetHealth(health.Health);
            }
        }
    }
}
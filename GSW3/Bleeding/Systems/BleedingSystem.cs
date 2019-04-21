using GSW3.GswWorld;
using GSW3.Health;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Bleeding.Systems
{
    [EcsInject]
    public class BleedingSystem : IEcsRunSystem
    {
        private const float HEAL_RATE_SLOWER = 100f;

        private readonly EcsWorld _ecsWorld = null;
        private readonly GameService _gameService = null;
        private readonly EcsFilter<GswPedComponent, HealthComponent, PedBleedingInfoComponent> _entities = null;

#if DEBUG
        private readonly GswLogger _logger;

        public BleedingSystem()
        {
            _logger = new GswLogger(typeof(BleedingSystem));
        }
#endif

        public void Run()
        {
            if(_gameService.GameIsPaused) return;
            
            foreach (int i in _entities)
            {
                Ped ped = _entities.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                HealthComponent health = _entities.Components2[i];
                if (health.Health <= 0) continue;

                PedBleedingInfoComponent info = _entities.Components3[i];
                EcsEntity pedEntity = _entities.Entities[i];
                float bleedingDamage = 0;

                for (int bleedingIndex = info.BleedingEntities.Count - 1; bleedingIndex >= 0; bleedingIndex--)
                {
                    EcsEntity bleedingEntity = info.BleedingEntities[bleedingIndex];
                    if (!_ecsWorld.IsEntityExists(bleedingEntity))
                    {
                        info.BleedingEntities.RemoveAt(bleedingIndex);
                        continue;
                    }

                    var bleeding = _ecsWorld.GetComponent<BleedingComponent>(bleedingEntity);
                    if (bleeding == null)
                    {
                        info.BleedingEntities.RemoveAt(bleedingIndex);
                        continue;
                    }

                    float delta = GswExtensions.GetDeltaTime();
                    if (delta <= 0) continue;

                    bleedingDamage += bleeding.Severity * delta;
                    bleeding.Severity -= info.BleedingHealRate / HEAL_RATE_SLOWER * delta;
                    if (bleeding.Severity > 0) continue;

#if DEBUG
                    _logger.MakeLog($"Bleeding {bleedingEntity} on {pedEntity.GetEntityName()} was healed");
#endif
                    _ecsWorld.RemoveComponent<BleedingComponent>(bleedingEntity);
                    info.BleedingEntities.RemoveAt(bleedingIndex);
                }

                if (bleedingDamage <= 0) continue;
                health.Health -= bleedingDamage;
                ped.SetHealth(health.Health);
            }
        }
    }
}
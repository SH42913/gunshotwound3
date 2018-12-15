using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.WoundProcessing.Health
{
    [EcsInject]
    public class HealDetectSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, HealthComponent> _pedsToCheck;

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
                HealthComponent health = _pedsToCheck.Components2[i];
                if (ped.Health <= health.MaxHealth) continue;
                
#if DEBUG
                _logger.MakeLog("Ped " + ped.Name() + " was fully healed with " + ped.Health + "/" + health.MaxHealth);
#endif
                _ecsWorld.AddComponent<FullyHealedComponent>(_pedsToCheck.Entities[i]);
            }
        }
    }
}
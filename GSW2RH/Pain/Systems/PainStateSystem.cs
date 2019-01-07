using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Pain.Systems
{
    [EcsInject]
    public class PainStateSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<PainStatsComponent> _woundStats;
        private EcsFilter<PainComponent, PainInfoComponent> _entities;

        private readonly GswLogger _logger;

        public PainStateSystem()
        {
            _logger = new GswLogger(typeof(PainStateSystem));
        }
        
        public void Run()
        {
            if(_woundStats.EntitiesCount <= 0) return;
            PainStatsComponent stats = _woundStats.Components1[0];
            
            foreach (int i in _entities)
            {
                PainComponent pain = _entities.Components1[i];
                PainInfoComponent painInfo = _entities.Components2[i];
                int entity = _entities.Entities[i];
                
                float painPercent = pain.PainAmount / painInfo.UnbearablePain;
                if (painPercent > stats.DeadlyPainMultiplier)
                {
                    CheckState(entity, painInfo, PainStates.DEADLY);
                }
                else if (painPercent > 1f)
                {
                    CheckState(entity, painInfo, PainStates.UNBEARABLE);
                }
                else if(painPercent > 0.7f)
                {
                    CheckState(entity, painInfo, PainStates.INTENSE);
                }
                else if (painPercent > 0.4f)
                {
                    CheckState(entity, painInfo, PainStates.AVERAGE);
                }
                else if (painPercent > 0.1f)
                {
                    CheckState(entity, painInfo, PainStates.MILD);
                }
                else
                {
                    CheckState(entity, painInfo, PainStates.NONE);
                }
            }
        }

        private void CheckState(int entity, PainInfoComponent painInfo, PainStates stateToCheck)
        {
            if(painInfo.CurrentPainState == stateToCheck) return;

#if DEBUG
            _logger.MakeLog($"Entity ({entity}): Changed Pain State from {painInfo.CurrentPainState} to {stateToCheck}");
#endif
            painInfo.CurrentPainState = stateToCheck;
            _ecsWorld.AddComponent<ChangedPainStateComponent>(entity).NewPainState = stateToCheck;
        }
    }
}
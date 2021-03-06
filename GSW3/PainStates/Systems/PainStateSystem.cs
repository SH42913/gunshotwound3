using GSW3.Notifications;
using GSW3.Pain;
using GSW3.Player;
using GSW3.Utils;
using GSW3.Wounds;
using Leopotam.Ecs;

namespace GSW3.PainStates.Systems
{
    [EcsInject]
    public class PainStateSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<PainComponent, PainInfoComponent, CurrentPainStateComponent> _entities = null;
        private readonly EcsFilter<PainIsGoneComponent, CurrentPainStateComponent> _painIsGone = null;
        private readonly EcsFilter<PainStateListComponent> _painStates = null;

#if DEBUG
        private readonly GswLogger _logger;

        public PainStateSystem()
        {
            _logger = new GswLogger(typeof(PainStateSystem));
        }
#endif

        public void Run()
        {
            PainStateListComponent stateList = _painStates.Components1[0];
            foreach (int i in _painIsGone)
            {
                _painIsGone.Components2[i].CurrentPainStateIndex = -1;

#if DEBUG
                EcsEntity pedEntity = _painIsGone.Entities[i];
                CurrentPainStateComponent stateComponent = _painIsGone.Components2[i];
                int oldState = stateComponent.CurrentPainStateIndex;
                int newStateIndex = stateComponent.CurrentPainStateIndex;
    
                string currentState = oldState >= 0
                    ? stateList.PainStateEntities[oldState].GetEntityName()
                    : "NO PAIN";
                
                string newState = newStateIndex >= 0
                    ? stateList.PainStateEntities[newStateIndex].GetEntityName()
                    : "NO PAIN";
                
                _logger.MakeLog($"{pedEntity.GetEntityName()}: Changed Pain State from {currentState} to {newState}");
#endif
            }

            foreach (int pedIndex in _entities)
            {
                EcsEntity pedEntity = _entities.Entities[pedIndex];
                PainComponent pain = _entities.Components1[pedIndex];
                PainInfoComponent painInfo = _entities.Components2[pedIndex];
                CurrentPainStateComponent currentStateComponent = _entities.Components3[pedIndex];

                int newStateIndex = -1;
                float painPercent = pain.PainAmount / painInfo.UnbearablePain;
                foreach (float percent in stateList.PainStatePercents)
                {
                    if (painPercent < percent) break;

                    newStateIndex++;
                }

                int currentStateIndex = currentStateComponent.CurrentPainStateIndex;
                if (currentStateIndex == newStateIndex) continue;

                int diff = newStateIndex - currentStateIndex;
                var wounded = _ecsWorld.EnsureComponent<WoundedComponent>(pedEntity, out _);
                bool isPlayer = _ecsWorld.GetComponent<PlayerMarkComponent>(pedEntity) != null;
                if (diff > 0)
                {
                    if (isPlayer)
                    {
                        _ecsWorld.CreateEntityWith(out NotificationComponent notification);
                        notification.Message += "~o~You feel yourself worst";
                    }
                    
                    for (int i = currentStateIndex + 1; i <= newStateIndex; i++)
                    {
                        EcsEntity newStateEntity = stateList.PainStateEntities[i];
#if DEBUG
                        _logger.MakeLog($"Added {newStateEntity.GetEntityName()} PainState");
#endif
                        wounded.WoundEntities.Add(newStateEntity);
                    }
                }
                else
                {
                    if (isPlayer)
                    {
                        _ecsWorld.CreateEntityWith(out NotificationComponent notification);
                        notification.Message += "~g~You feel yourself better";
                    }
                    
                    for (int i = currentStateIndex - 1; i >= newStateIndex; i--)
                    {
                        if (i < 0) continue;

                        EcsEntity newStateEntity = stateList.PainStateEntities[i];
#if DEBUG
                        _logger.MakeLog($"Added {newStateEntity.GetEntityName()} PainState");
#endif
                        wounded.WoundEntities.Add(newStateEntity);
                    }
                }

                currentStateComponent.CurrentPainStateIndex = newStateIndex;
#if DEBUG
                string currentState = currentStateIndex >= 0
                    ? stateList.PainStateEntities[currentStateIndex].GetEntityName()
                    : "NO PAIN";
                
                string newState = newStateIndex >= 0
                    ? stateList.PainStateEntities[newStateIndex].GetEntityName()
                    : "NO PAIN";
                _logger.MakeLog($"{pedEntity.GetEntityName()}: Changed Pain State from {currentState} to {newState}");
#endif
            }
        }
    }
}
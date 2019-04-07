using System;
using GunshotWound2.Pain;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;

namespace GunshotWound2.PainStates.Systems
{
    [EcsInject]
    public class PainStateSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<PainComponent, PainInfoComponent, CurrentPainStateComponent> _entities = null;
        private readonly EcsFilter<PainIsGoneComponent, CurrentPainStateComponent> _painIsGone = null;
        private readonly EcsFilter<PainStateListComponent> _painStates = null;

        private readonly GswLogger _logger;

        public PainStateSystem()
        {
            _logger = new GswLogger(typeof(PainStateSystem));
        }

        public void Run()
        {
            if (_painStates.IsEmpty())
            {
                throw new Exception("PainState list was not init!");
            }

            PainStateListComponent stateList = _painStates.Components1[0];
            foreach (int i in _painIsGone)
            {
                CurrentPainStateComponent stateComponent = _painIsGone.Components2[i];
                EcsEntity pedEntity = _painIsGone.Entities[i];
                int oldState = stateComponent.CurrentPainStateIndex;

                _painIsGone.Components2[i].CurrentPainStateIndex = -1;
                int newStateIndex = stateComponent.CurrentPainStateIndex;

#if DEBUG
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
                if (!_ecsWorld.IsEntityExists(pedEntity)) continue;

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
                if (diff > 0)
                {
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
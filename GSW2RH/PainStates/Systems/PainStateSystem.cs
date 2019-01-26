using System;
using GunshotWound2.Health;
using GunshotWound2.Pain;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;

namespace GunshotWound2.PainStates.Systems
{
    [EcsInject]
    public class PainStateSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<PainComponent, PainInfoComponent, CurrentPainStateComponent> _entities;
        private EcsFilter<PainIsGoneComponent, CurrentPainStateComponent> _painIsGone;
        private EcsFilter<PainStateListComponent> _painStates;

        private readonly GswLogger _logger;

        public PainStateSystem()
        {
            _logger = new GswLogger(typeof(PainStateSystem));
        }

        public void Run()
        {
            if (_painStates.EntitiesCount <= 0)
            {
                throw new Exception("PainState list was not init!");
            }

            foreach (int i in _painIsGone)
            {
                _painIsGone.Components2[i].CurrentPainStateIndex = -1;
            }

            PainStateListComponent stateList = _painStates.Components1[0];
            foreach (int pedEntity in _entities)
            {
                PainComponent pain = _entities.Components1[pedEntity];
                PainInfoComponent painInfo = _entities.Components2[pedEntity];
                CurrentPainStateComponent currentStateComponent = _entities.Components3[pedEntity];
                int entity = _entities.Entities[pedEntity];

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
                        int newStateEntity = stateList.PainStateEntities[i];
#if DEBUG
                        _logger.MakeLog($"Added {newStateEntity.GetEntityName(_ecsWorld)} PainState");
#endif
                        wounded.WoundEntities.Add(newStateEntity);
                    }
                }
                else
                {
                    for (int i = currentStateIndex - 1; i >= newStateIndex; i--)
                    {
                        if (i < 0) continue;

                        int newStateEntity = stateList.PainStateEntities[i];
#if DEBUG
                        _logger.MakeLog($"Added {newStateEntity.GetEntityName(_ecsWorld)} PainState");
#endif
                        wounded.WoundEntities.Add(newStateEntity);
                    }
                }

                currentStateComponent.CurrentPainStateIndex = newStateIndex;
#if DEBUG
                string currentState = currentStateIndex >= 0
                    ? stateList.PainStateEntities[currentStateIndex].GetEntityName(_ecsWorld)
                    : "NO PAIN";
                string newState = newStateIndex >= 0
                    ? stateList.PainStateEntities[newStateIndex].GetEntityName(_ecsWorld)
                    : "NO PAIN";
                _logger.MakeLog($"Entity ({entity}): Changed Pain State from {currentState} to {newState}");
#endif
            }
        }
    }
}
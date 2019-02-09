using System.Collections.Generic;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.PainStates.Systems
{
    [EcsInject]
    public class PainStateInitSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<LoadedConfigComponent> _loadedConfigs;
        private EcsFilter<PainStateComponent> _painStates;
        private EcsFilter<NewPedMarkComponent> _newPeds;

        private readonly GswLogger _logger;
        private const string PAIN_STATE_LIST = "PainStateList";

        public PainStateInitSystem()
        {
            _logger = new GswLogger(typeof(PainStateInitSystem));
        }

        public void PreInitialize()
        {
            _logger.MakeLog("Pain State list is loading!");

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                XElement listElement = xmlRoot.Element(PAIN_STATE_LIST);
                if (listElement == null) continue;

                foreach (XElement stateRoot in listElement.Elements("PainState"))
                {
                    CreatePainState(stateRoot);
                }
            }

            _logger.MakeLog("Pain State list loaded!");
        }

        private void CreatePainState(XElement stateRoot)
        {
            _ecsWorld.CreateEntityWith(out PainStateComponent state, out LoadedItemConfigComponent initComponent);
            state.PainPercent = stateRoot.GetFloat("PainPercent");
            initComponent.ElementRoot = stateRoot;
        }

        public void Initialize()
        {
            var component = _ecsWorld.AddComponent<PainStateListComponent>(GunshotWound2Script.StatsContainerEntity);

            var sortedDict = new SortedDictionary<float, int>();
            foreach (int i in _painStates)
            {
                PainStateComponent state = _painStates.Components1[i];
                int stateEntity = _painStates.Entities[i];
                sortedDict.Add(state.PainPercent, stateEntity);
            }

            foreach (KeyValuePair<float,int> pair in sortedDict)
            {
                component.PainStateEntities.Add(pair.Value);
                component.PainStatePercents.Add(pair.Key);
#if DEBUG
                _logger.MakeLog($"PainState {pair.Value.GetEntityName()} got {pair.Key} percent");
#endif
            }
        }

        public void Run()
        {
            foreach (int i in _newPeds)
            {
                int pedEntity = _newPeds.Entities[i];
                _ecsWorld.AddComponent<CurrentPainStateComponent>(pedEntity);
            }
        }

        public void PreDestroy()
        {
        }

        public void Destroy()
        {
        }
    }
}
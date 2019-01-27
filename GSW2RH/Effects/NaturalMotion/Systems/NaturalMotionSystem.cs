using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Effects.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, WoundedComponent> _woundedPeds;

        private readonly GswLogger _logger;

        public NaturalMotionSystem()
        {
            _logger = new GswLogger(typeof(NaturalMotionSystem));
        }

        public void Run()
        {
            foreach (int pedIndex in _woundedPeds)
            {
                Ped ped = _woundedPeds.Components1[pedIndex].ThisPed;
                if (!ped.Exists()) continue;

                WoundedComponent wounded = _woundedPeds.Components2[pedIndex];
                int pedEntity = _woundedPeds.Entities[pedIndex];
                foreach (int woundEntity in wounded.WoundEntities)
                {
                    var nmMessages = _ecsWorld.GetComponent<NaturalMotionMessagesComponent>(woundEntity);
                    if (nmMessages == null || nmMessages.MessageList.Count <= 0) continue;

                    NativeFunction.Natives.CREATE_NM_MESSAGE(true, 0);
                    NativeFunction.Natives.GIVE_PED_NM_MESSAGE(ped);
                    foreach (int message in nmMessages.MessageList)
                    {
                        NativeFunction.Natives.CREATE_NM_MESSAGE(true, message);
                        NativeFunction.Natives.GIVE_PED_NM_MESSAGE(ped);
                    }
#if DEBUG
                    _logger.MakeLog($"{ped.Name(pedEntity)} got {nmMessages}");
#endif
                }
            }
        }
    }
}
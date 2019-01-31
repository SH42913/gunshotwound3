using System;
using GunshotWound2.Effects.NaturalMotion.Arguments;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;
using Rage.Euphoria;
using Rage.Native;

namespace GunshotWound2.Effects.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, WoundedComponent> _woundedPeds;
        private EcsFilter<NaturalMotionMessagesDictComponent> _dict;

        private readonly GswLogger _logger;
        private static readonly Random Random = new Random();

        public NaturalMotionSystem()
        {
            _logger = new GswLogger(typeof(NaturalMotionSystem));
        }

        public void Run()
        {
            if (_dict.EntitiesCount <= 0)
            {
                throw new Exception("NaturalMotionSystem was not init");
            }

            var dict = _dict.Components1[0];
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
                    foreach (string messageName in nmMessages.MessageList)
                    {
                        NaturalMotionMessage nmMessage = dict.MessageDict[messageName];
                        
                        EuphoriaMessage message = new EuphoriaMessage(nmMessage.Name, true);
                        foreach (NmArgument argument in nmMessage.NmArguments)
                        {
                            message.SetArgument(argument.Name, argument.Value);
                        }

                        foreach (RandomFloatArgument argument in nmMessage.RandomFloatArguments)
                        {
                            message.SetArgument(argument.Name, Random.NextMinMax(argument.Value));
                        }

                        foreach (RandomIntArgument argument in nmMessage.RandomIntArguments)
                        {
                            message.SetArgument(argument.Name, Random.NextMinMax(argument.Value));
                        }
                        
                        message.SendTo(ped);
                    }
#if DEBUG
                    _logger.MakeLog($"{ped.Name(pedEntity)} got {nmMessages}");
#endif
                }
            }
        }
    }
}
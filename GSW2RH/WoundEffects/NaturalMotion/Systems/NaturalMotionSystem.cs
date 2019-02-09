using System;
using GunshotWound2.Utils;
using GunshotWound2.WoundEffects.NaturalMotion.Arguments;
using Leopotam.Ecs;
using Rage;
using Rage.Euphoria;
using Rage.Native;

namespace GunshotWound2.WoundEffects.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionSystem : BaseEffectSystem
    {
        private EcsFilter<NaturalMotionMessagesDictComponent> _dict;

        private static readonly Random Random = new Random();

        public NaturalMotionSystem() : base(new GswLogger(typeof(NaturalMotionSystem)))
        {
        }

        protected override void PrepareRunActions()
        {
            if (_dict.EntitiesCount <= 0)
            {
                throw new Exception("NaturalMotionSystem was not init");
            }
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            var dict = _dict.Components1[0];

            var nmMessages = EcsWorld.GetComponent<NaturalMotionMessagesComponent>(woundEntity);
            if (nmMessages == null || nmMessages.MessageList.Count <= 0) return;

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
            Logger.MakeLog($"{ped.Name(pedEntity)} got {nmMessages}");
#endif
        }
    }
}
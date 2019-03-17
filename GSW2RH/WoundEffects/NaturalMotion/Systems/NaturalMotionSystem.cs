using System;
using GunshotWound2.Utils;
using GunshotWound2.WoundEffects.NaturalMotion.Arguments;
using GunshotWound2.WoundEffects.Ragdoll;
using Leopotam.Ecs;
using Rage;
using Rage.Euphoria;
using Rage.Native;

namespace GunshotWound2.WoundEffects.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionSystem : BaseEffectSystem
    {
        private readonly EcsFilter<NaturalMotionMessagesDictComponent> _dict = null;
        private static readonly Random Random = new Random();

        public NaturalMotionSystem() : base(new GswLogger(typeof(NaturalMotionSystem)))
        {
        }

        protected override void PreExecuteActions()
        {
            if (_dict.IsEmpty())
            {
                throw new Exception("NaturalMotionSystem was not init");
            }
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            NaturalMotionMessagesDictComponent dict = _dict.Components1[0];

            var nmMessages = EcsWorld.GetComponent<NaturalMotionMessagesComponent>(woundEntity);
            if (nmMessages == null || nmMessages.MessageList.Count <= 0) return;

            var permanentRagdoll = EcsWorld.GetComponent<PermanentRagdollComponent>(pedEntity);
            if (permanentRagdoll != null && !permanentRagdoll.DisableOnlyOnHeal)
            {
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 0, 0, 1, 0, 0, 0);
            }
            
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
            Logger.MakeLog($"{pedEntity.GetEntityName()} have got {nmMessages}");
#endif
        }
    }
}
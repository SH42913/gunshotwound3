using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.NaturalMotion
{
    public class NaturalMotionMessagesComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck] 
        public readonly List<string> MessageList = new List<string>(16);

        public bool PlayInPermanentRagdoll;

        public void Reset()
        {
            MessageList.Clear();
        }

        public override string ToString()
        {
            string messages = nameof(MessageList);

            foreach (string i in MessageList)
            {
                messages += $" {i};";
            }

            return $"{nameof(NaturalMotionMessagesComponent)}: " + messages;
        }
    }
}
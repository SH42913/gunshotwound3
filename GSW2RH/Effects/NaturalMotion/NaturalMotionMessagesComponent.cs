using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.NaturalMotion
{
    public class NaturalMotionMessagesComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck] 
        public readonly List<string> MessageList = new List<string>();

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
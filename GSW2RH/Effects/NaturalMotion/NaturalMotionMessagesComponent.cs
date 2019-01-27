using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.NaturalMotion
{
    public class NaturalMotionMessagesComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck] public readonly List<int> MessageList = new List<int>();

        public void Reset()
        {
            MessageList.Clear();
        }

        public override string ToString()
        {
            string messages = nameof(MessageList);

            foreach (int i in MessageList)
            {
                messages += $" {i};";
            }

            return $"{nameof(NaturalMotionMessagesComponent)}: " + messages;
        }
    }
}
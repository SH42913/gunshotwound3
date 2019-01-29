using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.NaturalMotion
{
    public class NaturalMotionMessagesDictComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<string, NaturalMotionMessage> MessageDict = new Dictionary<string, NaturalMotionMessage>();
        
        public void Reset()
        {
            MessageDict.Clear();
        }
    }
}
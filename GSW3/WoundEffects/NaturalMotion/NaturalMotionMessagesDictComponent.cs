using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.NaturalMotion
{
    public class NaturalMotionMessagesDictComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<string, NaturalMotionMessage> MessageDict = new Dictionary<string, NaturalMotionMessage>(32);
        
        public void Reset()
        {
            MessageDict.Clear();
        }
    }
}
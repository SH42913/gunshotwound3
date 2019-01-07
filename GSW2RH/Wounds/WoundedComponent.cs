using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Wounds
{
    [EcsOneFrame]
    public class WoundedComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly List<int> WoundEntities = new List<int>();
        
        public void Reset()
        {
            WoundEntities.Clear();
        }
    }
}
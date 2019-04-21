using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.PedFlags
{
    public class ChangedPedFlagsComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly HashSet<int> ChangedFlags = new HashSet<int>();
        
        public void Reset()
        {
            ChangedFlags.Clear();
        }
    }
}
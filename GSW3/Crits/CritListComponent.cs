using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.Crits
{
    public class CritListComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly List<EcsEntity> CritList = new List<EcsEntity>(16);

        public void Reset()
        {
            CritList.Clear();
        }
    }
}
using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Crits
{
    public class CritListComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public List<EcsEntity> CritList = new List<EcsEntity>(16);

        public void Reset()
        {
            CritList.Clear();
        }
    }
}
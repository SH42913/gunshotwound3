using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Crits
{
    public class CritListComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public List<int> CritList = new List<int>();

        public void Reset()
        {
            CritList.Clear();
        }
    }
}
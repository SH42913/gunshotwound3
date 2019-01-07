using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Uids
{
    public class UidToEntityDictComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<long, int> UidToEntityDict = new Dictionary<long, int>();

        public void Reset()
        {
            UidToEntityDict.Clear();
        }
    }
}
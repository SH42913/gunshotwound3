using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Uids
{
    public class UidToEntityDictComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<string, int> UidToEntityDict = new Dictionary<string, int>();

        public void Reset()
        {
            UidToEntityDict.Clear();
        }
    }
}
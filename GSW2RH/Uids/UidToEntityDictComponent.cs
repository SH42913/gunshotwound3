using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Uids
{
    public class UidToEntityDictComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<string, EcsEntity> UidToEntityDict = new Dictionary<string, EcsEntity>(32);

        public void Reset()
        {
            UidToEntityDict.Clear();
        }
    }
}
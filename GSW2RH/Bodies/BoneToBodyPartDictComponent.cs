using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Bodies
{
    public class BoneToBodyPartDictComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<uint, int> BoneIdToBodyPartEntity = new Dictionary<uint, int>();
        
        public void Reset()
        {
            BoneIdToBodyPartEntity.Clear();
        }
    }
}
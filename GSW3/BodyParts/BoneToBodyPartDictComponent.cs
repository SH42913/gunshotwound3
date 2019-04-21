using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.BodyParts
{
    public class BoneToBodyPartDictComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<uint, EcsEntity> BoneIdToBodyPartEntity = new Dictionary<uint, EcsEntity>(64);
        
        public void Reset()
        {
            BoneIdToBodyPartEntity.Clear();
        }
    }
}
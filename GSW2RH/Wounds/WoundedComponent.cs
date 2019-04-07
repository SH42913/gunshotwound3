using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Wounds
{
    [EcsOneFrame]
    public class WoundedComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly List<EcsEntity> WoundEntities = new List<EcsEntity>(16);
        
        public void Reset()
        {
            WoundEntities.Clear();
        }
    }
}
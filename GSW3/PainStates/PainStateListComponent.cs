using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.PainStates
{
    public class PainStateListComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly List<EcsEntity> PainStateEntities = new List<EcsEntity>(8);
        
        [EcsIgnoreNullCheck]
        public readonly List<float> PainStatePercents = new List<float>();

        public void Reset()
        {
            PainStateEntities.Clear();
            PainStatePercents.Clear();
        }
    }
}
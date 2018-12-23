using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Bleeding
{
    public class BleedingInfoComponent : IEcsAutoResetComponent
    {
        public float BleedingHealRate;
        
        [EcsIgnoreNullCheck]
        public readonly List<int> BleedingEntities = new List<int>();
        
        public void Reset()
        {
            BleedingEntities.Clear();
        }
    }
}
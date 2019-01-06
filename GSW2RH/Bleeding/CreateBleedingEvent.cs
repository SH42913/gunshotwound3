using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Bleeding
{
    [EcsOneFrame]
    public class CreateBleedingEvent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Queue<float> BleedingToCreate = new Queue<float>();
        
        public void Reset()
        {
            BleedingToCreate.Clear();
        }
    }
}
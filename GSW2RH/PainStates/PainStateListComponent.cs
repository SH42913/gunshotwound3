using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.PainStates
{
    public class PainStateListComponent : IEcsAutoResetComponent
    {
        public readonly List<int> PainStateEntities = new List<int>();
        public readonly List<float> PainStatePercents = new List<float>();

        public void Reset()
        {
            PainStateEntities.Clear();
            PainStatePercents.Clear();
        }
    }
}
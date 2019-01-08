using Leopotam.Ecs;
using Weighted_Randomizer;

namespace GunshotWound2.Wounds
{
    public class WoundRandomizerComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly StaticWeightedRandomizer<int> WoundRandomizer = new StaticWeightedRandomizer<int>();

        public void Reset()
        {
            WoundRandomizer.Clear();
        }
    }
}
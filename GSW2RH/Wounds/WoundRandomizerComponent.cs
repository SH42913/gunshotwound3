using Leopotam.Ecs;
using Weighted_Randomizer;

namespace GunshotWound2.Wounds
{
    public class WoundRandomizerComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly StaticWeightedRandomizer<EcsEntity> WoundRandomizer = new StaticWeightedRandomizer<EcsEntity>();

        public void Reset()
        {
            WoundRandomizer.Clear();
        }
    }
}
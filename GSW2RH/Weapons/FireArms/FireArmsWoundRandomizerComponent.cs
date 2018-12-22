using Leopotam.Ecs;
using Weighted_Randomizer;

namespace GunshotWound2.Weapons.FireArms
{
    public class FireArmsWoundRandomizerComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly StaticWeightedRandomizer<FireArmsWounds> WoundRandomizer = new StaticWeightedRandomizer<FireArmsWounds>();

        public void Reset()
        {
            WoundRandomizer.Clear();
        }
    }
}
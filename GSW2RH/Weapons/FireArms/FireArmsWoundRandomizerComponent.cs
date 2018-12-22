using Leopotam.Ecs;
using Weighted_Randomizer;

namespace GunshotWound2.Weapons.FireArms
{
    public class FireArmsWoundRandomizerComponent : IEcsAutoResetComponent
    {
        public StaticWeightedRandomizer<FireArmsWounds> WoundRandomizer;

        public void Reset()
        {
            WoundRandomizer = null;
        }
    }
}
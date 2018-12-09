using Leopotam.Ecs;
using Weighted_Randomizer;

namespace GunshotWound2.Weapons.FireArms
{
    public enum FireArmsWounds
    {
        GRAZE_WOUND,
        FLESH_WOUND,
        PENETRATING_WOUND,
        PERFORATING_WOUND,
        AVULSIVE_WOUND
    }
    
    public class FireArmsWoundRandomizerComponent : IEcsAutoResetComponent
    {
        public StaticWeightedRandomizer<FireArmsWounds> WoundRandomizer;

        public void Reset()
        {
            WoundRandomizer = null;
        }
    }
}
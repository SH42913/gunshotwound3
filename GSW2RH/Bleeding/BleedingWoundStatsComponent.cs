namespace GunshotWound2.Bleeding
{
    public class BleedingWoundStatsComponent
    {
        public float BleedingMultiplier;
        public float BleedingDeviation;

        public override string ToString()
        {
            return $"{nameof(BleedingWoundStatsComponent)}: " +
                   $"{nameof(BleedingMultiplier)} {BleedingMultiplier}; " +
                   $"{nameof(BleedingDeviation)} {BleedingDeviation}; ";
        }
    }
}
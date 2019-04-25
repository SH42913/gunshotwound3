namespace GSW3.Bleeding
{
    public class BleedingStatsComponent
    {
        public float BleedingMultiplier;
        public float BleedingDeviation;
        public float BleedingDamageMultiplier;

        public override string ToString()
        {
            return $"{nameof(BleedingStatsComponent)}: " +
                   $"{nameof(BleedingMultiplier)} {BleedingMultiplier}; " +
                   $"{nameof(BleedingDeviation)} {BleedingDeviation}; " +
                   $"{nameof(BleedingDamageMultiplier)} {BleedingDamageMultiplier}; ";
        }
    }
}
namespace GunshotWound2.Bleeding
{
    public class BleedingStatsComponent
    {
        public float BleedingMultiplier;
        public float BleedingDeviation;

        public override string ToString()
        {
            return $"{nameof(BleedingStatsComponent)}: " +
                   $"{nameof(BleedingMultiplier)} {BleedingMultiplier}; " +
                   $"{nameof(BleedingDeviation)} {BleedingDeviation}; ";
        }
    }
}
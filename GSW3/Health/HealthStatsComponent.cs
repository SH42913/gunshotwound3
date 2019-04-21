namespace GSW3.Health
{
    public class HealthStatsComponent
    {
        public float DamageMultiplier;
        public float DamageDeviation;

        public override string ToString()
        {
            return
                $"{nameof(HealthStatsComponent)}: " +
                $"{nameof(DamageMultiplier)} {DamageMultiplier}; " +
                $"{nameof(DamageDeviation)} {DamageDeviation}; ";
        }
    }
}
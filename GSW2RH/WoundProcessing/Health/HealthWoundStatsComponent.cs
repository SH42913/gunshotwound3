namespace GunshotWound2.WoundProcessing.Health
{
    public class HealthWoundStatsComponent
    {
        public float DamageMultiplier;
        public float DamageDeviation;

        public override string ToString()
        {
            return
                $"{nameof(HealthWoundStatsComponent)}: " +
                $"{nameof(DamageMultiplier)} {DamageMultiplier}; " +
                $"{nameof(DamageDeviation)} {DamageDeviation}; ";
        }
    }
}
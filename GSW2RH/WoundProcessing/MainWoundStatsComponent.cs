namespace GunshotWound2.WoundProcessing
{
    public class MainWoundStatsComponent
    {
        public float DamageMultiplier;
        public float DamageDeviation;
        public float PainMultiplier;
        public float PainDeviation;
        public float DeadlyPainMultiplier;

        public override string ToString()
        {
            return
                $"{nameof(MainWoundStatsComponent)}: " +
                $"{nameof(DamageMultiplier)} {DamageMultiplier}; " +
                $"{nameof(DamageDeviation)} {DamageDeviation}; " +
                $"{nameof(PainMultiplier)} {PainMultiplier}; " +
                $"{nameof(PainDeviation)} {PainDeviation}; " +
                $"{nameof(DeadlyPainMultiplier)} {DeadlyPainMultiplier}; ";
        }
    }
}
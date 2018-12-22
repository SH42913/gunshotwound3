namespace GunshotWound2.WoundProcessing.Pain
{
    public class PainWoundStatsComponent
    {
        public float PainMultiplier;
        public float PainDeviation;
        public float DeadlyPainMultiplier;

        public override string ToString()
        {
            return
                $"{nameof(PainWoundStatsComponent)}: " +
                $"{nameof(PainMultiplier)} {PainMultiplier}; " +
                $"{nameof(PainDeviation)} {PainDeviation}; " +
                $"{nameof(DeadlyPainMultiplier)} {DeadlyPainMultiplier}; ";
        }
    }
}
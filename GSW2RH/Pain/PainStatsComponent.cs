namespace GunshotWound2.Pain
{
    public class PainStatsComponent
    {
        public float PainMultiplier;
        public float PainDeviation;
        public float DeadlyPainMultiplier;

        public override string ToString()
        {
            return
                $"{nameof(PainStatsComponent)}: " +
                $"{nameof(PainMultiplier)} {PainMultiplier}; " +
                $"{nameof(PainDeviation)} {PainDeviation}; " +
                $"{nameof(DeadlyPainMultiplier)} {DeadlyPainMultiplier}; ";
        }
    }
}
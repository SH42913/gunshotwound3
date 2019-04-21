namespace GSW3.Pain
{
    public class PainStatsComponent
    {
        public float PainMultiplier;
        public float PainDeviation;

        public override string ToString()
        {
            return
                $"{nameof(PainStatsComponent)}: " +
                $"{nameof(PainMultiplier)} {PainMultiplier}; " +
                $"{nameof(PainDeviation)} {PainDeviation}; ";
        }
    }
}
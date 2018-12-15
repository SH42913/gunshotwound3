namespace GunshotWound2.WoundProcessing
{
    public class MainWoundStatsComponent
    {
        public float DamageMultiplier;
        public float DamageDeviation;

        public override string ToString()
        {
            return nameof(MainWoundStatsComponent) + ": " +
                   nameof(DamageMultiplier) + " " + DamageMultiplier + "; " +
                   nameof(DamageDeviation) + " " + DamageDeviation + "; ";
        }
    }
}
namespace GunshotWound2.Bleeding
{
    public class BleedingMultiplierComponent
    {
        public float Multiplier;

        public override string ToString()
        {
            return nameof(BleedingMultiplierComponent) + ": " +
                   nameof(Multiplier) + " " + Multiplier + "; ";
        }
    }
}
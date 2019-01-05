namespace GunshotWound2.WoundProcessing.Bleeding
{
    public class BleedingMultComponent
    {
        public float Multiplier;

        public override string ToString()
        {
            return nameof(BleedingMultComponent) + ": " +
                   nameof(Multiplier) + " " + Multiplier + "; ";
        }
    }
}
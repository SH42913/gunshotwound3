namespace GSW3.Health
{
    public class DamageMultComponent
    {
        public float Multiplier;

        public override string ToString()
        {
            return nameof(DamageMultComponent) + ": " +
                   nameof(Multiplier) + " " + Multiplier + "; ";
        }
    }
}
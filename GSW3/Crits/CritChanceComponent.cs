namespace GSW3.Crits
{
    public class CritChanceComponent
    {
        public float CritChance;

        public override string ToString()
        {
            return nameof(CritChanceComponent) + ": " +
                   nameof(CritChance) + " " + CritChance + "; ";
        }
    }
}
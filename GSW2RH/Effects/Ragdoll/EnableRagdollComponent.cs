namespace GunshotWound2.Effects.Ragdoll
{
    public class EnableRagdollComponent
    {
        public int LengthInMs;
        public int Type;
        public bool Permanent;
        public bool DisableOnlyOnHeal;

        public override string ToString()
        {
            return nameof(EnableRagdollComponent) + ": " +
                   nameof(LengthInMs) + " " + LengthInMs + "; " +
                   nameof(Type) + " " + Type + "; " +
                   nameof(Permanent) + " " + Permanent + "; " +
                   nameof(DisableOnlyOnHeal) + " " + DisableOnlyOnHeal + "; ";
        }
    }
}
namespace GunshotWound2.Armor
{
    public class BodyPartArmorComponent
    {
        public bool ProtectedByHelmet;
        public bool ProtectedByBodyArmor;

        public override string ToString()
        {
            return nameof(BodyPartArmorComponent) + ": " +
                   nameof(ProtectedByHelmet) + " " + ProtectedByHelmet + "; " +
                   nameof(ProtectedByBodyArmor) + " " + ProtectedByBodyArmor + "; " ;
        }
    }
}
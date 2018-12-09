namespace GunshotWound2.Armor
{
    public class ArmorWeaponStatsComponent
    {
        public bool CanPenetrateArmor;
        public int ArmorDamage;
        public float MinArmorPercentForPenetration;
        public float ChanceToPenetrateHelmet;

        public override string ToString()
        {
            return nameof(ArmorWeaponStatsComponent) + ": " +
                   nameof(CanPenetrateArmor) + " " + CanPenetrateArmor + "; " +
                   nameof(ArmorDamage) + " " + ArmorDamage + "; " +
                   nameof(MinArmorPercentForPenetration) + " " + MinArmorPercentForPenetration + "; " +
                   nameof(ChanceToPenetrateHelmet) + " " + ChanceToPenetrateHelmet;
        }
    }
}
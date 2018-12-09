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
            return "ArmorWeaponStats: " +
                   "CanPenetrateArmor " + CanPenetrateArmor + ";" +
                   "ArmorDamage " + ArmorDamage + ";" +
                   "MinArmorPercentForPenetration " + MinArmorPercentForPenetration + ";" +
                   "ChanceToPenetrateHelmet " + ChanceToPenetrateHelmet;
        }
    }
}
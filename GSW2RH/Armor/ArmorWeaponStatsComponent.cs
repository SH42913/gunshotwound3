namespace GunshotWound2.Armor
{
    public class ArmorWeaponStatsComponent
    {
        public bool CanPenetrateArmor;
        public int ArmorDamage;
        public float PercentToPenetrateChance;
        public float HelmetSaveChance;

        public override string ToString()
        {
            return "ArmorWeaponStats: " +
                   "CanPenetrateArmor " + CanPenetrateArmor + ";" +
                   "ArmorDamage " + ArmorDamage + ";" +
                   "PercentToPenetrateChance " + PercentToPenetrateChance + ";" +
                   "HelmetSaveChance " + HelmetSaveChance;
        }
    }
}
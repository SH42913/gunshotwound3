namespace GunshotWound2.Weapons
{
    public class BaseWeaponStatsComponent
    {
        public float DamageMult;
        public float BleedingMult;
        public float PainMult;
        public float CritChance;

        public override string ToString()
        {
            return "BaseWeaponStats: " +
                   "DamageMult " + DamageMult + ";" +
                   "BleedingMult " + BleedingMult + ";" +
                   "PainMult " + PainMult + ";" +
                   "CritChance " + CritChance;
        }
    }
}
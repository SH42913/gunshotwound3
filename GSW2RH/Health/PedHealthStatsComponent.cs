using GunshotWound2.Utils;

namespace GunshotWound2.Health
{
    public class PedHealthStatsComponent
    {
        public MinMax PedHealth;
        public float AnimalMult;
        public float PlayerHealth;

        public override string ToString()
        {
            return $"{nameof(PedHealthStatsComponent)}: " +
                   $"{nameof(PedHealth)} {PedHealth}; " +
                   $"{nameof(AnimalMult)} {AnimalMult}; " +
                   $"{nameof(PlayerHealth)} {PlayerHealth}; ";
        }
    }
}
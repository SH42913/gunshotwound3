using GSW3.Utils;

namespace GSW3.Health
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
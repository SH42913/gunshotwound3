using GSW3.Utils;

namespace GSW3.Bleeding
{
    public class PedBleedingStatsComponent
    {
        public MinMax PedBleedingHealRate;
        public float AnimalMult;
        public float PlayerBleedingHealRate;

        public override string ToString()
        {
            return $"{nameof(PedBleedingStatsComponent)}: " +
                   $"{nameof(PedBleedingHealRate)} {PedBleedingHealRate}; " +
                   $"{nameof(AnimalMult)} {AnimalMult}; " +
                   $"{nameof(PlayerBleedingHealRate)} {PlayerBleedingHealRate}; ";
        }
    }
}
using GunshotWound2.Utils;

namespace GunshotWound2.Bleeding
{
    public class PedBleedingStatsComponent
    {
        public MinMax PedBleedingHealRate;

        public override string ToString()
        {
            return $"{nameof(PedBleedingStatsComponent)}: " +
                   $"{nameof(PedBleedingHealRate)} {PedBleedingHealRate}; ";
        }
    }
}
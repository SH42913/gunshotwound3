using GunshotWound2.Utils;

namespace GunshotWound2.WoundProcessing.Health
{
    public class PedHealthStatsComponent
    {
        public MinMax PedHealth;

        public override string ToString()
        {
            return $"{nameof(PedHealthStatsComponent)}: " +
                   $"{nameof(PedHealth)} {PedHealth}; ";
        }
    }
}
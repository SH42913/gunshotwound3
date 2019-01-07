using GunshotWound2.Utils;

namespace GunshotWound2.Health
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
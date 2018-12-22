using GunshotWound2.Utils;

namespace GunshotWound2.WoundProcessing.Pain
{
    public class PedPainStatsComponent
    {
        public MinMax PedUnbearablePain;
        public MinMax PedPainRecoverySpeed;

        public override string ToString()
        {
            return $"{nameof(PedPainStatsComponent)}: " +
                   $"{nameof(PedUnbearablePain)} {PedUnbearablePain}; " +
                   $"{nameof(PedPainRecoverySpeed)} {PedPainRecoverySpeed}; ";
        }
    }
}
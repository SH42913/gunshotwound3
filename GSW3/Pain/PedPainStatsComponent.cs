using GSW3.Utils;

namespace GSW3.Pain
{
    public class PedPainStatsComponent
    {
        public MinMax PedUnbearablePain;
        public MinMax PedPainRecoverySpeed;
        public float PlayerUnbearablePain;
        public float AnimalMult;
        public float PlayerPainRecoverySpeed;

        public override string ToString()
        {
            return $"{nameof(PedPainStatsComponent)}: " +
                   $"{nameof(PedUnbearablePain)} {PedUnbearablePain}; " +
                   $"{nameof(PedPainRecoverySpeed)} {PedPainRecoverySpeed}; " +
                   $"{nameof(PlayerUnbearablePain)} {PlayerUnbearablePain}; " +
                   $"{nameof(AnimalMult)} {AnimalMult}; " +
                   $"{nameof(PlayerPainRecoverySpeed)} {PlayerPainRecoverySpeed}; ";
        }
    }
}
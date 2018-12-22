using System.Collections.Generic;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld
{
    public class GswWorldComponent : IEcsAutoResetComponent
    {
        public bool PedDetectingEnabled;
        public bool AnimalDetectingEnabled;

        public MinMax PedHealth;
        public MinMax PedAccuracy;
        public MinMax PedShootRate;

        public MinMax PedUnbearablePain;
        public MinMax PedPainRecoverySpeed;

        public bool ScanOnlyDamaged;

        [EcsIgnoreNullCheck]
        public readonly Queue<Ped> NeedToCheckPeds = new Queue<Ped>();
        
        [EcsIgnoreNullCheck]
        public readonly Dictionary<Ped, int> PedsToEntityDict = new Dictionary<Ped, int>();

        public int MaxDetectTimeInMs;

        public void Reset()
        {
            NeedToCheckPeds.Clear();
            PedsToEntityDict.Clear();
        }

        public override string ToString()
        {
            return
                $"{nameof(GswWorldComponent)}: " +
                $"{nameof(PedDetectingEnabled)} {PedDetectingEnabled}; " +
                $"{nameof(AnimalDetectingEnabled)} {AnimalDetectingEnabled}; " +
                $"{nameof(PedHealth)} {PedHealth}; " +
                $"{nameof(PedAccuracy)} {PedAccuracy}; " +
                $"{nameof(PedShootRate)} {PedShootRate}; " +
                $"{nameof(PedUnbearablePain)} {PedUnbearablePain}; " +
                $"{nameof(PedPainRecoverySpeed)} {PedPainRecoverySpeed}; " +
                $"{nameof(ScanOnlyDamaged)} {ScanOnlyDamaged}; " +
                $"{nameof(MaxDetectTimeInMs)} {MaxDetectTimeInMs}";
        }
    }
}
using System.Collections.Generic;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld
{
    public class GswWorldComponent : IEcsAutoResetComponent
    {
        public bool HumanDetectingEnabled;
        public bool AnimalDetectingEnabled;

        public MinMax HumanAccuracy;
        public MinMax HumanShootRate;

        public bool ScanOnlyDamaged;

        [EcsIgnoreNullCheck]
        public readonly Queue<Ped> NeedToCheckPeds = new Queue<Ped>();
        
        [EcsIgnoreNullCheck]
        public readonly HashSet<Ped> ForceCreatePeds = new HashSet<Ped>();

        [EcsIgnoreNullCheck]
        public readonly Dictionary<Ped, int> PedsToEntityDict = new Dictionary<Ped, int>();

        public int MaxDetectTimeInMs;

        public void Reset()
        {
            NeedToCheckPeds.Clear();
            ForceCreatePeds.Clear();
            PedsToEntityDict.Clear();
        }

        public override string ToString()
        {
            return $"{nameof(GswWorldComponent)}: " +
                   $"{nameof(HumanDetectingEnabled)} {HumanDetectingEnabled}; " +
                   $"{nameof(AnimalDetectingEnabled)} {AnimalDetectingEnabled}; " +
                   $"{nameof(HumanAccuracy)} {HumanAccuracy}; " +
                   $"{nameof(HumanShootRate)} {HumanShootRate}; " +
                   $"{nameof(ScanOnlyDamaged)} {ScanOnlyDamaged}; " +
                   $"{nameof(MaxDetectTimeInMs)} {MaxDetectTimeInMs}";
        }
    }
}
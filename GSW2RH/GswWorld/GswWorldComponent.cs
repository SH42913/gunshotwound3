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

        public bool ScanOnlyDamaged;

        public Queue<Ped> NeedToCheckPeds;
        public Dictionary<Ped, int> PedsToEntityDict;

        public int MaxDetectTimeInMs;

        public void Reset()
        {
            NeedToCheckPeds = null;
            PedsToEntityDict = null;
        }

        public override string ToString()
        {
            return nameof(GswWorldComponent) + ": " +
                   nameof(PedDetectingEnabled) + " " + PedDetectingEnabled + "; " +
                   nameof(AnimalDetectingEnabled) + " " + AnimalDetectingEnabled + "; " +
                   nameof(PedHealth) + " " + PedHealth + "; " +
                   nameof(PedAccuracy) + " " + PedAccuracy + "; " +
                   nameof(PedShootRate) + " " + PedShootRate + "; " +
                   nameof(ScanOnlyDamaged) + " " + ScanOnlyDamaged + "; " +
                   nameof(MaxDetectTimeInMs) + " " + MaxDetectTimeInMs;
        }
    }
}
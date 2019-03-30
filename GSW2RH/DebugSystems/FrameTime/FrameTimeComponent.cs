using System.Diagnostics;
using Leopotam.Ecs;

namespace GunshotWound2.DebugSystems.FrameTime
{
    public class FrameTimeComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Stopwatch Stopwatch = new Stopwatch();
        public long MaxFrameTime;
        
        public void Reset()
        {
            Stopwatch.Reset();
        }
    }
}
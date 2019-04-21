using System.Diagnostics;
using Leopotam.Ecs;

namespace GSW3.DebugSystems.FrameTime
{
#if DEBUG
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
#endif
}
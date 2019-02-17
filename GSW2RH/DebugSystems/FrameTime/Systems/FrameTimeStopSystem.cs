using System.Windows.Forms;
using GunshotWound2.DebugSystems.DebugText;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.DebugSystems.FrameTime.Systems
{
    [EcsInject]
    public class FrameTimeStopSystem : IEcsRunSystem
    {
        private readonly EcsFilter<FrameTimeComponent> _frameTime = null;
        private readonly EcsFilter<DebugTextComponent> _debugText = null;

        private const string FRAME_TIME_KEY = "Total/MaxTime";

        public void Run()
        {
            if (_frameTime.IsEmpty() || _debugText.IsEmpty()) return;

            FrameTimeComponent frameTime = _frameTime.Components1[0];
            frameTime.Stopwatch.Stop();
            long elapsed = frameTime.Stopwatch.ElapsedMilliseconds;
            if (Game.IsKeyDown(Keys.End))
            {
                frameTime.MaxFrameTime = 0;
            }
            else if (elapsed > frameTime.MaxFrameTime)
            {
                frameTime.MaxFrameTime = elapsed;
            }
            frameTime.Stopwatch.Reset();

            string frameTimeText = $"{elapsed}/{frameTime.MaxFrameTime}";
            _debugText.Components1[0].UpdateDebugText(FRAME_TIME_KEY, frameTimeText);
        }
    }
}
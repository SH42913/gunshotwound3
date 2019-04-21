using System.Windows.Forms;
using GSW3.DebugSystems.DebugText;
using GSW3.GswWorld;
using GSW3.Player;
using Leopotam.Ecs;
using Rage;

namespace GSW3.DebugSystems.FrameTime.Systems
{
#if DEBUG
    [EcsInject]
    public class FrameTimeStopSystem : IEcsRunSystem
    {
        private readonly EcsFilter<NewPedMarkComponent, PlayerMarkComponent> _newPlayers = null;
        private readonly EcsFilter<FrameTimeComponent> _frameTime = null;
        private readonly EcsFilter<DebugTextComponent> _debugText = null;

        private const string FRAME_TIME_KEY = "Total/MaxTime";

        public void Run()
        {
            if (_frameTime.IsEmpty() || _debugText.IsEmpty()) return;

            FrameTimeComponent frameTime = _frameTime.Components1[0];
            frameTime.Stopwatch.Stop();
            long elapsed = frameTime.Stopwatch.ElapsedMilliseconds;
            if (Game.IsKeyDown(Keys.End) || !_newPlayers.IsEmpty())
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
#endif
}
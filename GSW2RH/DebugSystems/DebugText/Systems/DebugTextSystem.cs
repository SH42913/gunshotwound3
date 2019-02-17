using System.Collections.Generic;
using System.Drawing;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.DebugSystems.DebugText.Systems
{
    [EcsInject]
    public class DebugTextSystem : IEcsPreInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<DebugTextComponent> _debugText = null;

        private const float SCREEN_HEIGHT_PERCENT = 0.8f;
        private const float SCREEN_WIDTH_PERCENT = 0.17f;

        public void PreInitialize()
        {
            _ecsWorld.AddComponent<DebugTextComponent>(GunshotWound2Script.StatsContainerEntity);
            Game.RawFrameRender += GameOnRawFrameRender;
        }

        private void GameOnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            if (_debugText.IsEmpty() || Game.IsPaused || Game.IsLoading) return;

            Dictionary<string, string> dict = _debugText.Components1[0].DebugKeyToText;
            if (dict.Count <= 0) return;

            string totalText = "";
            foreach (KeyValuePair<string, string> pair in dict)
            {
                totalText += $"{pair.Key}: {pair.Value}\n";
            }

            var textPosition = new PointF
            {
                X = Game.Resolution.Width * SCREEN_WIDTH_PERCENT,
                Y = Game.Resolution.Height * SCREEN_HEIGHT_PERCENT
            };
            e.Graphics.DrawText(totalText, "Arial", 15f, textPosition, Color.White);
        }

        public void PreDestroy()
        {
            Game.RawFrameRender -= GameOnRawFrameRender;
        }
    }
}
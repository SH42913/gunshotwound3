using System.Collections.Generic;
using System.Drawing;
using GunshotWound2.Pause;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.DebugSystems.DebugText.Systems
{
    [EcsInject]
    public class DebugTextSystem : IEcsPreInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<DebugTextComponent> _debugText = null;
        private readonly EcsFilter<PauseStateComponent> _pause = null;

        private const float SCREEN_HEIGHT_PERCENT = 0.8f;
        private const float SCREEN_WIDTH_PERCENT = 0.17f;
        private string _textToShow;
        private bool _registered;

        public void PreInitialize()
        {
            _ecsWorld.AddComponent<DebugTextComponent>(GunshotWound2Script.StatsContainerEntity);
            RegisterFrameRender();
        }

        public void Run()
        {
            if (_debugText.IsEmpty())
            {
                _textToShow = null;
                return;
            }

            Dictionary<string, string> dict = _debugText.Components1[0].DebugKeyToText;
            if (dict.Count <= 0)
            {
                _textToShow = null;
                return;
            }

            string totalText = "";
            foreach (KeyValuePair<string, string> pair in dict)
            {
                totalText += $"{pair.Key}: {pair.Value}\n";
            }

            _textToShow = totalText;
        }

        private void GameOnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            if (string.IsNullOrEmpty(_textToShow) || _pause.GameIsPaused()) return;

            var textPosition = new PointF
            {
                X = Game.Resolution.Width * SCREEN_WIDTH_PERCENT,
                Y = Game.Resolution.Height * SCREEN_HEIGHT_PERCENT
            };
            e.Graphics.DrawText(_textToShow, "Arial", 15f, textPosition, Color.White);
        }

        public void PreDestroy()
        {
            UnregisterFrameRender();
        }

        private void RegisterFrameRender()
        {
            if (!_registered)
            {
                Game.RawFrameRender += GameOnRawFrameRender;
                _registered = true;
            }
        }

        private void UnregisterFrameRender()
        {
            if (_registered)
            {
                Game.RawFrameRender -= GameOnRawFrameRender;
                _registered = false;
            }
        }
    }
}
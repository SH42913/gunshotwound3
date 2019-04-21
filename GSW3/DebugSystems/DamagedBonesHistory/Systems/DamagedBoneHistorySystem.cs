using System.Collections.Generic;
using System.Drawing;
using GSW3.GswWorld;
using Leopotam.Ecs;
using Rage;

namespace GSW3.DebugSystems.DamagedBonesHistory.Systems
{
#if DEBUG
    [EcsInject]
    public class DamagedBoneHistorySystem : IEcsPreInitSystem, IEcsRunSystem
    {
        private readonly EcsFilter<GswPedComponent, DamagedBoneHistoryComponent> _peds = null;
        private readonly GameService _gameService = null;

        private const float MAXIMAL_RANGE = 50f;

        private List<Vector2> _screenPositionList;
        private List<float> _radiusList;
        private List<Color> _colorList;
        private bool _registered;

        public void PreInitialize()
        {
            RegisterFrameRender();
        }

        public void Run()
        {
            if (_peds.IsEmpty())
            {
                _screenPositionList = null;
                _radiusList = null;
                _colorList = null;
                return;
            }

            Vector3 playerPosition = Game.LocalPlayer.Character.Position;
            var screenPositionList = new List<Vector2>();
            var radiusList = new List<float>();
            var colorList = new List<Color>();

            foreach (int i in _peds)
            {
                Ped ped = _peds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                PedBoneId?[] history = _peds.Components2[i].LastDamagedBones;
                for (int historyIndex = 0; historyIndex < history.Length; historyIndex++)
                {
                    PedBoneId? boneId = history[historyIndex];
                    if (boneId == null || !ped.IsBoneValid(boneId.Value)) continue;

                    PedBoneId bone = boneId.Value;
                    Vector3 position = ped.GetBonePosition(bone);
                    float distance = Vector3.Distance(playerPosition, position);
                    if (distance >= MAXIMAL_RANGE) continue;

                    Color color;
                    float radius;
                    switch (historyIndex)
                    {
                        case 0:
                            color = Color.DarkOrange;
                            radius = 15f;
                            break;
                        case 1:
                            color = Color.Yellow;
                            radius = 10f;
                            break;
                        case 2:
                            color = Color.ForestGreen;
                            radius = 5f;
                            break;
                        default:
                            continue;
                    }

                    Vector2 screenPosition = World.ConvertWorldPositionToScreenPosition(position);
                    float ratio = 1 - distance / MAXIMAL_RANGE;
                    radius *= ratio;

                    screenPositionList.Add(screenPosition);
                    radiusList.Add(radius);
                    colorList.Add(color);
                }
            }

            _screenPositionList = screenPositionList;
            _radiusList = radiusList;
            _colorList = colorList;
        }

        private void GameOnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            if (_screenPositionList == null || _screenPositionList.Count <= 0 || _gameService.GameIsPaused) return;

            for (int i = 0; i < _screenPositionList.Count; i++)
            {
                Vector2 screenPosition = _screenPositionList[i];
                float radius = _radiusList[i];
                Color color = _colorList[i];

                e.Graphics.DrawFilledCircle(screenPosition, radius, color);
            }
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
#endif
}
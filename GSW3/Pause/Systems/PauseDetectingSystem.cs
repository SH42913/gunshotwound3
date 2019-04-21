using Leopotam.Ecs;
using Rage;

namespace GSW3.Pause.Systems
{
    [EcsInject]
    public class PauseDetectingSystem : IEcsRunSystem
    {
        private readonly GameService _gameService = null;
        private uint _last;
        private bool _isPaused;

        public void Run()
        {
            _isPaused = Game.GameTime == _last;
            _last = Game.GameTime;
            _gameService.GameIsPaused = _isPaused;
        }
    }
}
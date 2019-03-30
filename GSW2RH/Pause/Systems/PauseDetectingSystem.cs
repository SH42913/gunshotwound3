using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Pause.Systems
{
    [EcsInject]
    public class PauseDetectingSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private uint _last;
        private bool _isPaused;

        public void Run()
        {
            _isPaused = Game.GameTime == _last;
            _last = Game.GameTime;

            int mainEntity = GunshotWound2Script.StatsContainerEntity;
            if (_isPaused)
            {
                _ecsWorld.EnsureComponent<PauseStateComponent>(mainEntity, out _);
            }
            else
            {
                _ecsWorld.RemoveComponent<PauseStateComponent>(mainEntity, true);
            }
        }
    }
}
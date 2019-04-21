using Leopotam.Ecs;

namespace GSW3.DebugSystems.FrameTime.Systems
{
#if DEBUG
    [EcsInject]
    public class FrameTimeStartSystem : IEcsPreInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<FrameTimeComponent> _frameTime = null;

        public void PreInitialize()
        {
            _ecsWorld.AddComponent<FrameTimeComponent>(GunshotWound3.StatsContainerEntity);
        }

        public void Run()
        {
            if (_frameTime.IsEmpty()) return;

            FrameTimeComponent frameTime = _frameTime.Components1[0];
            frameTime.Stopwatch.Start();
        }

        public void PreDestroy()
        {
        }
    }
#endif
}
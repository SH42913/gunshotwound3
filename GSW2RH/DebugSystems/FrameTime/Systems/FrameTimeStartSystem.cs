using Leopotam.Ecs;

namespace GunshotWound2.DebugSystems.FrameTime.Systems
{
    [EcsInject]
    public class FrameTimeStartSystem : IEcsPreInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<FrameTimeComponent> _frameTime = null;

        public void PreInitialize()
        {
            _ecsWorld.AddComponent<FrameTimeComponent>(GunshotWound2Script.StatsContainerEntity);
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
}
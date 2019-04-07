using GunshotWound2.Health;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Bleeding.Systems
{
    [EcsInject]
    public class BleedingHealSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<PedBleedingInfoComponent, FullyHealedComponent> _healedEntities = null;

        public void Run()
        {
            foreach (int i in _healedEntities)
            {
                PedBleedingInfoComponent info = _healedEntities.Components1[i];
                foreach (EcsEntity bleedingEntity in info.BleedingEntities)
                {
                    if (!_ecsWorld.IsEntityExists(bleedingEntity)) continue;

                    _ecsWorld.RemoveEntity(bleedingEntity);
                }
            }
        }
    }
}
using GSW3.Utils;
using GSW3.Weapons;
using Leopotam.Ecs;

namespace GSW3.Wounds.Systems
{
    [EcsInject]
    public class WoundSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly EcsFilter<DamagedByWeaponComponent> _damagedPeds = null;

        private readonly GswLogger _logger;

        public WoundSystem()
        {
            _logger = new GswLogger(typeof(WoundSystem));
        }

        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                DamagedByWeaponComponent damagedComponent = _damagedPeds.Components1[i];
                EcsEntity weaponEntity = damagedComponent.WeaponEntity;
                EcsEntity pedEntity = _damagedPeds.Entities[i];

                var woundRandomizer = _ecsWorld.GetComponent<WoundRandomizerComponent>(weaponEntity);
                if (woundRandomizer == null || woundRandomizer.WoundRandomizer.Count <= 0)
                {
                    _logger.MakeLog($"!!!WARNING!!! Weapon {weaponEntity.GetEntityName()} " +
                                    $"doesn't have {nameof(WoundRandomizerComponent)}");
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

                EcsEntity woundEntity = woundRandomizer.WoundRandomizer.NextWithReplacement();
                damagedComponent.MainWoundEntity = woundEntity;

                var wounded = _ecsWorld.EnsureComponent<WoundedComponent>(pedEntity, out _);
                wounded.WoundEntities.Add(woundEntity);

#if DEBUG
                _logger.MakeLog($"{pedEntity.GetEntityName()} have got wound {woundEntity.GetEntityName()}");
#endif
            }
        }
    }
}
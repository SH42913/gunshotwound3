using GunshotWound2.Utils;
using GunshotWound2.Weapons;
using Leopotam.Ecs;

namespace GunshotWound2.Wounds.Systems
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
                int weaponEntity = damagedComponent.WeaponEntity;
                int pedEntity = _damagedPeds.Entities[i];

                var woundRandomizer = _ecsWorld.GetComponent<WoundRandomizerComponent>(weaponEntity);
                if (woundRandomizer == null || woundRandomizer.WoundRandomizer.Count <= 0)
                {
                    _logger.MakeLog($"Weapon {weaponEntity.GetEntityName()} " +
                                    $"doesn't have {nameof(WoundRandomizerComponent)}");
                    _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(pedEntity);
                    continue;
                }

                int woundEntity = woundRandomizer.WoundRandomizer.NextWithReplacement();
                damagedComponent.WoundEntity = woundEntity;

                var wounded = _ecsWorld.EnsureComponent<WoundedComponent>(pedEntity, out _);
                wounded.WoundEntities.Add(woundEntity);

#if DEBUG
                _logger.MakeLog($"{pedEntity.GetEntityName()} have got wound {woundEntity.GetEntityName()}");
#endif
            }
        }
    }
}
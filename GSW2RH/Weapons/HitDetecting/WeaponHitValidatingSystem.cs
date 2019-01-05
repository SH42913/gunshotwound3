using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Weapons.HitDetecting
{
    [EcsInject]
    public class WeaponHitValidatingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<DamagedByWeaponComponent> _hits;

        private readonly GswLogger _logger;

        public WeaponHitValidatingSystem()
        {
            _logger = new GswLogger(typeof(WeaponHitValidatingSystem));
        }

        public void Run()
        {
            foreach (int i in _hits)
            {
                int hitEntity = _hits.Entities[i];
                int weaponEntity = _hits.Components1[i].WeaponEntity;
                WeaponTypes type = _hits.Components1[i].WeaponType;

                if (_ecsWorld.IsEntityExists(weaponEntity)) continue;
                
                _logger.MakeLog($"Weapon Entity with type {type} doesn\'t exist");
                _ecsWorld.RemoveComponent<DamagedByWeaponComponent>(hitEntity);
            }
        }
    }
}
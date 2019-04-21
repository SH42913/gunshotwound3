using Leopotam.Ecs;

namespace GSW3.Weapons
{
    [EcsOneFrame]
    public class DamagedByWeaponComponent
    {
        public EcsEntity WeaponEntity;
        public EcsEntity MainWoundEntity;
    }
}
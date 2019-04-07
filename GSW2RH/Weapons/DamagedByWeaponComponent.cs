using Leopotam.Ecs;

namespace GunshotWound2.Weapons
{
    [EcsOneFrame]
    public class DamagedByWeaponComponent
    {
        public EcsEntity WeaponEntity;
        public EcsEntity WoundEntity;
    }
}
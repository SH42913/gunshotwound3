using Leopotam.Ecs;

namespace GunshotWound2.Weapons
{
    [EcsOneFrame]
    public class DamagedByWeaponComponent
    {
        public int WeaponEntity;
        public int WoundEntity;
    }
}
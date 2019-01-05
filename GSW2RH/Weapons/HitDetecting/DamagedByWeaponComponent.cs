using Leopotam.Ecs;

namespace GunshotWound2.Weapons.HitDetecting
{
    [EcsOneFrame]
    public class DamagedByWeaponComponent
    {
        public int WeaponEntity;
        public WeaponTypes WeaponType;
    }
}
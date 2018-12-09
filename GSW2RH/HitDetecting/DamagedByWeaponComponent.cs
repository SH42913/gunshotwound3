using GunshotWound2.Weapons;
using Leopotam.Ecs;

namespace GunshotWound2.HitDetecting
{
    [EcsOneFrame]
    public class DamagedByWeaponComponent
    {
        public int WeaponEntity;
        public WeaponTypes WeaponType;
    }
}
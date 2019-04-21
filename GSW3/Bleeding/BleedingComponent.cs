using Leopotam.Ecs;

namespace GSW3.Bleeding
{
    public class BleedingComponent
    {
        public uint DamagedBoneId;
        public float Severity;
        public EcsEntity MotherWoundEntity;
        public EcsEntity WeaponEntity;
    }
}
using Leopotam.Ecs;

namespace GSW3.Bleeding
{
    public class BleedingComponent
    {
        public float Severity;
        public EcsEntity MotherWoundEntity;
        public EcsEntity WeaponEntity;
        public uint DamagedBoneId;
        public EcsEntity BodyPartEntity;
    }
}
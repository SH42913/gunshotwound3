using Leopotam.Ecs;

namespace GSW3.BodyParts
{
    [EcsOneFrame]
    public class DamagedBodyPartComponent
    {
        public uint DamagedBoneId;
        public EcsEntity DamagedBodyPartEntity;
    }
}
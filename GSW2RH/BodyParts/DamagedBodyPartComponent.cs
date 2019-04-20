using Leopotam.Ecs;

namespace GunshotWound2.BodyParts
{
    [EcsOneFrame]
    public class DamagedBodyPartComponent
    {
        public uint DamagedBoneId;
        public EcsEntity DamagedBodyPartEntity;
    }
}
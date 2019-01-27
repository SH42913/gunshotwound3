using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.BodyParts
{
    public class BodyHitHistoryComponent
    {
        [EcsIgnoreNullCheck]
        public readonly PedBoneId?[] LastDamagedBones = new PedBoneId?[3];
    }
}
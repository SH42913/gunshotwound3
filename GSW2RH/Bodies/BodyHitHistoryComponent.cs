using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Bodies
{
    public class BodyHitHistoryComponent
    {
        [EcsIgnoreNullCheck]
        public readonly PedBoneId?[] LastDamagedBones = new PedBoneId?[3];
    }
}
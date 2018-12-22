using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.HitDetecting
{
    public class BodyHitHistoryComponent
    {
        [EcsIgnoreNullCheck]
        public readonly PedBoneId?[] LastDamagedBones = new PedBoneId?[3];
    }
}
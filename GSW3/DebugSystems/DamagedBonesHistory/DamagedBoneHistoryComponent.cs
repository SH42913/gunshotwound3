using Leopotam.Ecs;
using Rage;

namespace GSW3.DebugSystems.DamagedBonesHistory
{
#if DEBUG
    public class DamagedBoneHistoryComponent
    {
        [EcsIgnoreNullCheck]
        public readonly PedBoneId?[] LastDamagedBones = new PedBoneId?[3];
    }
#endif
}
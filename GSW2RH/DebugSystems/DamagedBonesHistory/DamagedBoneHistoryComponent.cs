using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.DebugSystems.DamagedBonesHistory
{
    public class DamagedBoneHistoryComponent
    {
        [EcsIgnoreNullCheck]
        public readonly PedBoneId?[] LastDamagedBones = new PedBoneId?[3];
    }
}
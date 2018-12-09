using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.HitDetecting
{
    public class BodyHitHistoryComponent : IEcsAutoResetComponent
    {
        public PedBoneId?[] LastDamagedBones;
        
        public void Reset()
        {
            LastDamagedBones = null;
        }
    }
}
using Leopotam.Ecs;
using Rage;

namespace GSW3.GswWorld
{
    public class ForceCreateGswPedEvent : IEcsAutoResetComponent
    {
        public Ped TargetPed;
        
        public void Reset()
        {
            TargetPed = null;
        }
    }
}
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld
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
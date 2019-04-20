using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld
{
    public class GswPedComponent : IEcsAutoResetComponent
    {
        public Ped ThisPed;
        public float DefaultAccuracy;

        public void Reset()
        {
            ThisPed = null;
        }
    }
}
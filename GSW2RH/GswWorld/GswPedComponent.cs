using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.GswWorld
{
    public class GswPedComponent : IEcsAutoResetComponent
    {
        public Ped ThisPed;

        public int DefaultAccuracy;
        public int Armor;

        public string NoPainAnim;
        public string MildPainAnim;
        public string AvgPainAnim;
        public string IntensePainAnim;

        public void Reset()
        {
            ThisPed = null;
        }
    }
}
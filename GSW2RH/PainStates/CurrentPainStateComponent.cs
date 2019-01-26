using Leopotam.Ecs;

namespace GunshotWound2.PainStates
{
    public class CurrentPainStateComponent : IEcsAutoResetComponent
    {
        public int CurrentPainStateIndex = -1;

        public void Reset()
        {
            CurrentPainStateIndex = -1;
        }
    }
}
using Leopotam.Ecs;

namespace GSW3.PainStates
{
    public class CurrentPainStateComponent : IEcsAutoResetComponent
    {
        public int CurrentPainStateIndex;

        public void Reset()
        {
            CurrentPainStateIndex = -1;
        }
    }
}
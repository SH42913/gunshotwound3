using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Pain
{
    [EcsOneFrame]
    public class ChangedPainStateComponent
    {
        public PainStates NewPainState;
    }
}
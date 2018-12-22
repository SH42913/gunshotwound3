using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Pain
{
    [EcsOneFrame]
    public class ReceivedPainComponent : IEcsAutoResetComponent
    {
        public float Pain;
        
        public void Reset()
        {
            Pain = 0;
        }
    }
}
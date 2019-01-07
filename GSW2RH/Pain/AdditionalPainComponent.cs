using Leopotam.Ecs;

namespace GunshotWound2.Pain
{
    [EcsOneFrame]
    public class AdditionalPainComponent : IEcsAutoResetComponent
    {
        public float AdditionalPain;
        
        public void Reset()
        {
            AdditionalPain = 0;
        }
    }
}
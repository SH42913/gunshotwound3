using Leopotam.Ecs;

namespace GSW3.Pain
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
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing.Health
{
    [EcsOneFrame]
    public class ReceivedDamageComponent : IEcsAutoResetComponent
    {
        public float Damage;
        
        public void Reset()
        {
            Damage = 0;
        }
    }
}
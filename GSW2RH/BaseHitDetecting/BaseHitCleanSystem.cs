using GunshotWound2.GswWorld;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.BaseHitDetecting
{
    [EcsInject]
    public class BaseHitCleanSystem : IEcsRunSystem
    {
        private EcsFilter<GswPedComponent, HasBeenHitMarkComponent> _hitPeds;

        public void Run()
        {
            foreach (int i in _hitPeds)
            {
                Ped ped = _hitPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                NativeFunction.Natives.CLEAR_PED_LAST_WEAPON_DAMAGE(ped);
            }
        }
    }
}
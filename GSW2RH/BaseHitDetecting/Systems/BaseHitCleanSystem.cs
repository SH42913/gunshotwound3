using GunshotWound2.GswWorld;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.BaseHitDetecting.Systems
{
    [EcsInject]
    public class BaseHitCleanSystem : IEcsRunSystem
    {
        private readonly EcsFilter<GswPedComponent, HasBeenHitMarkComponent> _hitPeds = null;

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
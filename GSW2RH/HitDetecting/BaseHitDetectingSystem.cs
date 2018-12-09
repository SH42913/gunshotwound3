using GunshotWound2.GswWorld;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.HitDetecting
{
    [EcsInject]
    public class BaseHitDetectingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent> _peds;
        
        public void Run()
        {
            foreach (int i in _peds)
            {
                Ped ped = _peds.Components1[i].ThisPed;
                if(!ped.Exists()) continue;
                
                bool damaged = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_PED<bool>(ped);
                if(!damaged) continue;

#if DEBUG
                Game.Console.Print("Ped " + ped.Model.Name + " has been damaged");
#endif
                _ecsWorld.AddComponent<HasBeenHitMarkComponent>(_peds.Entities[i]);
            }
        }
    }
}
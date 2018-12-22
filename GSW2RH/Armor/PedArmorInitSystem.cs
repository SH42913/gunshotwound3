using GunshotWound2.GswWorld;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Armor
{
    [EcsInject]
    public class PedArmorInitSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newPeds;
        
        public void Run()
        {
            foreach (int i in _newPeds)
            {
                Ped ped = _newPeds.Components1[i].ThisPed;
                int pedEntity = _newPeds.Entities[i];

                var armor = _ecsWorld.AddComponent<ArmorComponent>(pedEntity);
                armor.Armor = ped.Armor;
            }
        }
    }
}
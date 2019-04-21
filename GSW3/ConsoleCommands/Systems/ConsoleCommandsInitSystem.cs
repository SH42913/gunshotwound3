using GSW3.Bleeding;
using GSW3.GswWorld;
using Leopotam.Ecs;
using Rage;

namespace GSW3.ConsoleCommands.Systems
{
    [EcsInject]
    public class ConsoleCommandsInitSystem : IEcsInitSystem
    {
        private readonly EcsFilter<GswPedComponent, PedBleedingInfoComponent> _pedsWithBleedings = null;
        
        public void Initialize()
        {
            //Register some filters
            _pedsWithBleedings.IsEmpty();
            Game.AddConsoleCommands();
        }

        public void Destroy()
        {
        }
    }
}
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.ConsoleCommands.Systems
{
    public class ConsoleCommandsInitSystem : IEcsInitSystem
    {
        public void Initialize()
        {
            Game.AddConsoleCommands();
        }

        public void Destroy()
        {
        }
    }
}
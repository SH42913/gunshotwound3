using GunshotWound2.Player;
using Leopotam.Ecs;
using Rage;
using Rage.Attributes;

namespace GunshotWound2.Health
{
    public static class HealthConsoleCommands
    {
#if DEBUG
        [ConsoleCommand("GSW3: Remove all effects from player")]
        private static void Command_RemoveAllEffectsFromPlayer()
        {
            EcsWorld world = GunshotWound2Script.EcsWorld;
            var filter = world.GetFilter<EcsFilter<PlayerMarkComponent>>();
            if (filter.IsEmpty())
            {
                Game.Console.Print("There is no player in GSW3!");
                return;
            }

            EcsEntity playerEntity = filter.Entities[0];
            world.EnsureComponent<FullyHealedComponent>(playerEntity, out _);
        }
#endif
    }
}
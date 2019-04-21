using System;
using System.Collections.Generic;
using GSW3.WoundEffects.NaturalMotion.Systems;
using Leopotam.Ecs;
using Rage;
using Rage.Attributes;
using Rage.Native;

namespace GSW3.WoundEffects.NaturalMotion
{
    public class NaturalMotionMessageConsoleCommands
    {
#if DEBUG
        [ConsoleCommand("GSW3: Get NM message list")]
        private static void Command_GetNaturalMotionMessageList()
        {
            EcsWorld world = GunshotWound3.EcsWorld;
            var filter = world.GetFilter<EcsFilter<NaturalMotionMessagesDictComponent>>();
            if (filter.IsEmpty())
            {
                Game.Console.Print("Natural Motion System was not init!");
                return;
            }

            Dictionary<string, NaturalMotionMessage> dict = filter.Components1[0].MessageDict;
            if (dict.Count <= 0)
            {
                Game.Console.Print("Natural Motion System was not init!");
                return;
            }

            Game.Console.Print("NaturalMotionMessageList:");
            foreach (string key in dict.Keys)
            {
                Game.Console.Print(key);
            }
        }

        [ConsoleCommand("GSW3: Test NM message")]
        private static void Command_PlayNaturalMotionMessage(
            [ConsoleCommandParameter("NM-name")] string name,
            [ConsoleCommandParameter("Ragdoll Type")] int ragdollType,
            [ConsoleCommandParameter("Ragdoll Length in ms")] int ragdollLength)
        {
            EcsWorld world = GunshotWound3.EcsWorld;
            var filter = world.GetFilter<EcsFilter<NaturalMotionMessagesDictComponent>>();
            if (filter.IsEmpty())
            {
                Game.Console.Print("Natural Motion System was not init!");
                return;
            }

            Dictionary<string, NaturalMotionMessage> dict = filter.Components1[0].MessageDict;
            if (dict.Count <= 0 || !dict.ContainsKey(name))
            {
                Game.Console.Print($"Message {name} not exists!");
                return;
            }

            var ped = Game.LocalPlayer.Character;
            NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, ragdollLength, ragdollLength, ragdollType, 0, 0, 0);

            NaturalMotionMessage nmMessage = dict[name];
            NaturalMotionSystem.PlayNaturalMotionMessage(nmMessage, ped, new Random());
        }
#endif
    }
}
using System;
using GunshotWound2.Player;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Attributes;

namespace GunshotWound2.Bleeding
{
    public static class BleedingConsoleCommands
    {
#if DEBUG
        [ConsoleCommand("GSW3: Check bleedings of player")]
        private static void Command_GetPlayerBleedingList()
        {
            EcsWorld world = GunshotWound2Script.EcsWorld;
            var filter = world.GetFilter<EcsFilter<PlayerMarkComponent>>();
            if (filter.IsEmpty())
            {
                Game.Console.Print("There is no player in GSW3!");
                return;
            }

            EcsEntity playerEntity = filter.Entities[0];
            var info = world.GetComponent<PedBleedingInfoComponent>(playerEntity);
            if (info == null || info.BleedingEntities.Count <= 0)
            {
                Game.Console.Print("Player don't have any bleedings!");
                return;
            }

            Game.Console.Print("Player Bleedings:");
            foreach (EcsEntity bleedingEntity in info.BleedingEntities)
            {
                PrintInfoAboutBleeding(world, bleedingEntity, info);
            }
        }
        
        [ConsoleCommand("GSW3: Heal all bleedings of player")]
        private static void Command_HealPlayerBleedings()
        {
            EcsWorld world = GunshotWound2Script.EcsWorld;
            var filter = world.GetFilter<EcsFilter<PlayerMarkComponent>>();
            if (filter.IsEmpty())
            {
                Game.Console.Print("There is no player in GSW3!");
                return;
            }

            EcsEntity playerEntity = filter.Entities[0];
            var info = world.GetComponent<PedBleedingInfoComponent>(playerEntity);
            if (info == null || info.BleedingEntities.Count <= 0)
            {
                Game.Console.Print("Player don't have any bleedings!");
                return;
            }

            int bleedingsCount = info.BleedingEntities.Count;
            foreach (EcsEntity bleedingEntity in info.BleedingEntities)
            {
                world.RemoveEntity(bleedingEntity);
            }
            info.BleedingEntities.Clear();
            
            Game.Console.Print($"Healed {bleedingsCount} bleedings of player!");
        }

        [ConsoleCommand("GSW3: Check bleedings of all wounded peds")]
        private static void Command_GetBleedingListForAllWoundedPeds()
        {
            EcsWorld world = GunshotWound2Script.EcsWorld;
            var filter = world.GetFilter<EcsFilter<PedBleedingInfoComponent>>();
            if (filter.IsEmpty())
            {
                Game.Console.Print("There are no peds with bleedings!");
                return;
            }

            foreach (int i in filter)
            {
                PedBleedingInfoComponent info = filter.Components1[i];
                if (info.BleedingEntities.Count <= 0)
                {
                    return;
                }

                EcsEntity pedEntity = filter.Entities[i];
                Game.Console.Print($"Ped {pedEntity.GetEntityName()} has bleedings:");
                foreach (EcsEntity bleedingEntity in info.BleedingEntities)
                {
                    PrintInfoAboutBleeding(world, bleedingEntity, info);
                }
            }
        }

        private static void PrintInfoAboutBleeding(EcsWorld world, EcsEntity bleedingEntity, PedBleedingInfoComponent info)
        {
            var bleeding = world.GetComponent<BleedingComponent>(bleedingEntity);
            if (bleeding == null) return;
            
            string boneName = bleeding.DamagedBoneId.ToString();
            if (Enum.TryParse(boneName, out PedBoneId boneId))
            {
                boneName = boneId.ToString();
            }

            float estTime = bleeding.Severity / info.BleedingHealRate * 100f;
            Game.Console.Print($"Bleeding {bleeding.MotherWoundEntity.GetEntityName()} on {boneName} " +
                               $"by weapon {bleeding.WeaponEntity.GetEntityName()}, " +
                               $"severity {bleeding.Severity}, time to heal {estTime} seconds");
        }
#endif
    }
}
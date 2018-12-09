using System.Diagnostics;
using System.Drawing;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;
using Debug = Rage.Debug;

namespace GunshotWound2.GswWorld
{
    [EcsInject]
    public class GswWorldSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;

        private EcsFilter<GswWorldComponent> _world;
        private EcsFilter<GswPedComponent> _gswPeds;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void Run()
        {
            if (_world.EntitiesCount <= 0) return;

            GswWorldComponent gswWorld = _world.Components1[0];
            if (!gswWorld.PedDetectingEnabled) return;

            _stopwatch.Restart();
            if (gswWorld.NeedToCheckPeds.Count <= 0)
            {
                CleanWorld();

                foreach (Ped ped in World.GetAllPeds())
                {
                    gswWorld.NeedToCheckPeds.Enqueue(ped);
                }

                if (TimeIsOver()) return;
            }

            while (gswWorld.NeedToCheckPeds.Count > 0)
            {
                if (TimeIsOver()) break;

                Ped pedToCheck = gswWorld.NeedToCheckPeds.Dequeue();
                if (PedIsNotExistsDeadOrNotHuman(pedToCheck)) continue;
                if (CheckGswPedAlreadyExist(pedToCheck)) continue;
                if (CheckNotDamaged(pedToCheck)) continue;

                int entity = _ecsWorld.CreateEntityWith(out GswPedComponent gswPed);
                gswPed.ThisPed = pedToCheck;
                gswPed.DefaultAccuracy = pedToCheck.Accuracy;
                
                gswWorld.PedsToEntityDict.Add(pedToCheck, entity);
            }

#if DEBUG
            string pedCounter = "Peds: " + _gswPeds.EntitiesCount;
            pedCounter.ShowInGsw(0.165f, 0.94f, 0.25f, Color.White);

            string worldTime = "World Time: " + _stopwatch.ElapsedMilliseconds;
            worldTime.ShowInGsw(0.165f, 0.955f, 0.25f, Color.White);
#endif
            _stopwatch.Stop();
        }

        private bool TimeIsOver()
        {
            return _stopwatch.ElapsedMilliseconds > _world.Components1[0].MaxDetectTimeInMs;
        }

        private bool PedIsNotExistsDeadOrNotHuman(Ped ped)
        {
            return !ped.Exists() || !ped.IsHuman || ped.IsDead;
        }

        private bool CheckGswPedAlreadyExist(Ped pedToCheck)
        {
            return _world.Components1[0].PedsToEntityDict.ContainsKey(pedToCheck);
        }

        private bool CheckNotDamaged(Ped pedToCheck)
        {
            if (!_world.Components1[0].ScanOnlyDamaged) return false;

            bool damaged = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_PED<bool>(pedToCheck);
            return !damaged;
        }

        private void CleanWorld()
        {
            GswWorldComponent gswWorld = _world.Components1[0];

            foreach (int i in _gswPeds)
            {
                GswPedComponent gswPed = _gswPeds.Components1[i];

                Ped ped = gswPed.ThisPed;
                if (ped.Exists() && ped.IsAlive)
                {              
#if DEBUG
                    Debug.DrawSphereDebug(ped.AbovePosition + 0.4f * Vector3.WorldUp, 0.15f, Color.Red);
#endif
                    continue;
                }

                gswWorld.PedsToEntityDict.Remove(ped);
                _ecsWorld.RemoveEntity(_gswPeds.Entities[i]);
            }
        }
    }
}
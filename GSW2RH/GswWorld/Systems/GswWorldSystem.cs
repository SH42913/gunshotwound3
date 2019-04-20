using System;
using System.Diagnostics;
using GunshotWound2.Pause;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;
#if DEBUG
using GunshotWound2.DebugSystems.DebugText;

#endif

namespace GunshotWound2.GswWorld.Systems
{
    [EcsInject]
    public class GswWorldSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly Random _random = null;

        private readonly EcsFilter<GswWorldComponent> _world = null;
        private readonly EcsFilter<GswPedComponent> _gswPeds = null;
        private readonly EcsFilter<ForceCreateGswPedEvent> _forceCreateEvents = null;
        private readonly EcsFilter<PauseStateComponent> _pause = null;
#if DEBUG
        private readonly EcsFilter<DebugTextComponent> _debugText = null;
#endif

        private readonly GswLogger _logger;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public GswWorldSystem()
        {
            _logger = new GswLogger(typeof(GswWorldSystem));
        }

        public void Run()
        {
            if (_pause.GameIsPaused()) return;

            GswWorldComponent gswWorld = _world.Components1[0];
            bool detectingEnabled = gswWorld.HumanDetectingEnabled || gswWorld.AnimalDetectingEnabled;

            _stopwatch.Restart();
            if (gswWorld.NeedToCheckPeds.Count <= 0)
            {
                foreach (int i in _forceCreateEvents)
                {
                    Ped targetPed = _forceCreateEvents.Components1[i].TargetPed;
                    if (gswWorld.ForceCreatePeds.Contains(targetPed)) continue;

                    gswWorld.ForceCreatePeds.Add(targetPed);
                    gswWorld.NeedToCheckPeds.Enqueue(targetPed);
                    _ecsWorld.RemoveEntity(_forceCreateEvents.Entities[i]);
                }

                foreach (int i in _gswPeds)
                {
                    GswPedComponent gswPed = _gswPeds.Components1[i];

                    Ped ped = gswPed.ThisPed;
                    if (IsExistsAndAlive(ped)) continue;

                    EcsEntity pedEntity = _gswPeds.Entities[i];
                    _ecsWorld.AddComponent<RemovedPedMarkComponent>(pedEntity);
                }

                if (detectingEnabled)
                {
                    foreach (Ped ped in World.GetAllPeds())
                    {
                        gswWorld.NeedToCheckPeds.Enqueue(ped);
                    }
                }
            }

            while (!TimeIsOver() && gswWorld.NeedToCheckPeds.Count > 0)
            {
                Ped ped = gswWorld.NeedToCheckPeds.Dequeue();
                if (IsNotExistsOrDead(ped)) continue;
                if (GswPedAlreadyExist(ped)) continue;

                bool forceCreatePed = gswWorld.ForceCreatePeds.Contains(ped);
                if (!forceCreatePed && IsNotDamaged(ped)) continue;

                EcsEntity? entity = null;
                if (ped.IsHuman && (forceCreatePed || gswWorld.HumanDetectingEnabled))
                {
                    entity = CreateHuman(gswWorld, ped);
                }
                else if (!ped.IsHuman && (forceCreatePed || gswWorld.AnimalDetectingEnabled))
                {
                    entity = CreateAnimal(gswWorld, ped);
                }

                if (entity == null)
                {
                    throw new Exception($"Ped is not supported! {ped.Model.Name} is not animal or human!");
                }
                
                NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(ped, false);
                NativeFunction.Natives.SET_PED_CONFIG_FLAG(ped, 281, true);
                _ecsWorld.AddComponent<NewPedMarkComponent>(entity.Value);
                if (!forceCreatePed) continue;

#if DEBUG
                _logger.MakeLog($"Ped {entity.Value.GetEntityName()} was force created");
#endif
                gswWorld.ForceCreatePeds.Remove(ped);
            }

#if DEBUG
            if (!_debugText.IsEmpty())
            {
                _debugText.Components1[0].UpdateDebugText("Peds", _gswPeds.GetEnumerator().GetCount().ToString());
            }
#endif
            _stopwatch.Stop();
        }

        private bool TimeIsOver()
        {
            return _stopwatch.ElapsedMilliseconds > _world.Components1[0].MaxDetectTimeInMs;
        }

        private bool IsNotExistsOrDead(Ped ped)
        {
            return !ped.Exists() || ped.IsDead;
        }

        private bool IsExistsAndAlive(Ped ped)
        {
            return ped.Exists() && ped.IsAlive;
        }

        private bool GswPedAlreadyExist(Ped pedToCheck)
        {
            return _world.Components1[0].PedsToEntityDict.ContainsKey(pedToCheck);
        }

        private bool IsNotDamaged(Ped pedToCheck)
        {
            if (!_world.Components1[0].ScanOnlyDamaged) return false;

            bool damaged = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_PED<bool>(pedToCheck);
            return !damaged;
        }

        private EcsEntity CreateHuman(GswWorldComponent gswWorld, Ped ped)
        {
            EcsEntity entity = _ecsWorld.CreateEntityWith(out GswPedComponent gswPed);
            gswPed.ThisPed = ped;

            if (!gswWorld.HumanAccuracy.IsDisabled())
            {
                ped.Accuracy = (int) _random.NextMinMax(gswWorld.HumanAccuracy);
            }

            if (!gswWorld.HumanShootRate.IsDisabled())
            {
                int rate = (int) _random.NextMinMax(gswWorld.HumanShootRate);
                NativeFunction.Natives.SET_PED_SHOOT_RATE(ped, rate);
            }

            gswPed.DefaultAccuracy = ped.Accuracy;
            gswWorld.PedsToEntityDict.Add(ped, entity);
            return entity;
        }

        private EcsEntity CreateAnimal(GswWorldComponent gswWorld, Ped ped)
        {
            EcsEntity entity = _ecsWorld.CreateEntityWith(out GswPedComponent gswPed, out AnimalMarkComponent _);
            gswPed.ThisPed = ped;
            gswPed.DefaultAccuracy = ped.Accuracy;
            gswWorld.PedsToEntityDict.Add(ped, entity);
            return entity;
        }
    }
}
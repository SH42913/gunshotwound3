using GSW3.GswWorld;
using GSW3.Health;
using GSW3.Utils;
using GSW3.Wounds;
using Leopotam.Ecs;
using Rage;

namespace GSW3.WoundEffects
{
    [EcsInject]
    public abstract class BaseEffectSystem : IEcsRunSystem
    {
        protected EcsWorld EcsWorld;
        protected EcsFilter<GswPedComponent, WoundedComponent> WoundedPeds;
        protected EcsFilter<GswPedComponent, NewPedMarkComponent>.Exclude<WoundedComponent> NewPeds;
        protected EcsFilter<GswPedComponent, FullyHealedComponent> HealedPeds;

        protected readonly GswLogger Logger;

        protected BaseEffectSystem(GswLogger logger)
        {
            Logger = logger;
        }

        public void Run()
        {
            PreExecuteActions();

            foreach (int i in NewPeds)
            {
                Ped ped = NewPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                EcsEntity pedEntity = NewPeds.Entities[i];
                ResetEffect(ped, pedEntity);
            }

            foreach (int i in HealedPeds)
            {
                Ped ped = HealedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                EcsEntity pedEntity = HealedPeds.Entities[i];
                ResetEffect(ped, pedEntity);
            }

            foreach (int pedIndex in WoundedPeds)
            {
                Ped ped = WoundedPeds.Components1[pedIndex].ThisPed;
                if (!ped.Exists()) continue;

                EcsEntity pedEntity = WoundedPeds.Entities[pedIndex];
                WoundedComponent wounded = WoundedPeds.Components2[pedIndex];

                foreach (EcsEntity woundEntity in wounded.WoundEntities)
                {
                    ProcessWound(ped, pedEntity, woundEntity);
                }
            }
            
            PostExecuteActions();
        }

        protected virtual void PreExecuteActions()
        {
        }

        protected abstract void ResetEffect(Ped ped, EcsEntity pedEntity);

        protected abstract void ProcessWound(Ped ped, EcsEntity pedEntity, EcsEntity woundEntity);

        protected virtual void PostExecuteActions()
        {
        }
    }
}
using GunshotWound2.GswWorld;
using GunshotWound2.Health;
using GunshotWound2.Utils;
using GunshotWound2.Wounds;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.WoundEffects
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
            PrepareRunActions();

            foreach (int i in NewPeds)
            {
                Ped ped = NewPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = NewPeds.Entities[i];
                ResetEffect(ped, pedEntity);
            }

            foreach (int i in HealedPeds)
            {
                Ped ped = HealedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = HealedPeds.Entities[i];
                ResetEffect(ped, pedEntity);
            }

            foreach (int pedIndex in WoundedPeds)
            {
                Ped ped = WoundedPeds.Components1[pedIndex].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = WoundedPeds.Entities[pedIndex];
                WoundedComponent wounded = WoundedPeds.Components2[pedIndex];

                foreach (int woundEntity in wounded.WoundEntities)
                {
                    ProcessWound(ped, pedEntity, woundEntity);
                }
            }
        }

        protected abstract void PrepareRunActions();

        protected abstract void ResetEffect(Ped ped, int pedEntity);

        protected abstract void ProcessWound(Ped ped, int pedEntity, int woundEntity);
    }
}
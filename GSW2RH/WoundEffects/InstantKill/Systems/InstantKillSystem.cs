using GunshotWound2.Health;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.WoundEffects.InstantKill.Systems
{
    [EcsInject]
    public class InstantKillSystem : BaseEffectSystem
    {
        public InstantKillSystem() : base(new GswLogger(typeof(InstantKillSystem)))
        {
        }

        protected override void PrepareRunActions()
        {
        }

        protected override void ResetEffect(Ped ped, int pedEntity)
        {
        }

        protected override void ProcessWound(Ped ped, int pedEntity, int woundEntity)
        {
            var ikComponent = EcsWorld.GetComponent<InstantKillComponent>(woundEntity);
            var health = EcsWorld.GetComponent<HealthComponent>(pedEntity);
            if (ikComponent == null || health == null) return;

            int newHealth = (int) health.MinHealth - 2;
            ped.SetHealth(newHealth);
#if DEBUG
            Logger.MakeLog($"{pedEntity.GetEntityName()} was killed due {ikComponent.ReasonKey}!");
#endif
        }
    }
}
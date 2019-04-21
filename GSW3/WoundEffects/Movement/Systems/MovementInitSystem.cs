using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.Movement.Systems
{
    [EcsInject]
    public class MovementInitSystem : BaseEffectInitSystem
    {
        public MovementInitSystem() : base(new GswLogger(typeof(MovementInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement disable = partRoot.Element("DisableSprint");
            if (disable != null)
            {
                var component = EcsWorld.AddComponent<DisableSprintComponent>(partEntity);
                component.RestoreOnlyOnHeal = disable.GetBool("RestoreOnlyOnHeal");
            }

            XElement enable = partRoot.Element("EnableSprint");
            if (enable != null)
            {
                EcsWorld.AddComponent<EnableSprintComponent>(partEntity);
            }

            XElement newRate = partRoot.Element("MovementRate");
            if (newRate != null)
            {
                var component = EcsWorld.AddComponent<NewMovementRateComponent>(partEntity);
                component.Rate = newRate.GetFloat();
                component.RestoreOnlyOnHeal = newRate.GetBool("RestoreOnlyOnHeal");
            }

            XElement restore = partRoot.Element("RestoreMovementRate");
            if (restore != null)
            {
                EcsWorld.AddComponent<RestoreMovementComponent>(partEntity);
            }
        }
    }
}
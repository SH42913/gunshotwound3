using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.Movement.Systems
{
    [EcsInject]
    public class MovementInitSystem : BaseEffectInitSystem
    {
        public MovementInitSystem() : base(new GswLogger(typeof(MovementInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
        {
            XElement disable = partRoot.Element("DisableSprint");
            if (disable != null)
            {
                var component = EcsWorld.AddComponent<DisableSprintComponent>(partEntity);
                component.Permanent = disable.GetBool("Permanent");
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
                component.Permanent = newRate.GetBool("Permanent");
            }

            XElement restore = partRoot.Element("RestoreMovementRate");
            if (restore != null)
            {
                EcsWorld.AddComponent<RestoreMovementComponent>(partEntity);
            }
        }
    }
}
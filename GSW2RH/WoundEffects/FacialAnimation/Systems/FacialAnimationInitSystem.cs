using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.FacialAnimation.Systems
{
    [EcsInject]
    public class FacialAnimationInitSystem : BaseEffectInitSystem
    {
        public FacialAnimationInitSystem() : base(new GswLogger(typeof(FacialAnimationInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement enable = partRoot.Element("EnableFacialAnimation");
            if (enable != null)
            {
                var component = EcsWorld.AddComponent<EnableFacialAnimationComponent>(partEntity);
                component.MaleDict = enable.GetAttributeValue("MaleDict");
                component.FemaleDict = enable.GetAttributeValue("FemaleDict");
                component.Permanent = enable.GetBool("Permanent");

                foreach (string animation in enable.GetAttributeValue("Animations").Split(';'))
                {
                    if (!string.IsNullOrEmpty(animation))
                    {
                        component.Animations.Add(animation);
                    }
                }

#if DEBUG
                Logger.MakeLog($"{partEntity.GetEntityName()} have got {component}");
#endif
            }

            XElement disable = partRoot.Element("DisableFacialAnimation");
            if (disable != null)
            {
                EcsWorld.AddComponent<DisableFacialAnimationComponent>(partEntity);

#if DEBUG
                Logger.MakeLog($"{partEntity.GetEntityName()} will disable facial animation");
#endif
            }
        }
    }
}
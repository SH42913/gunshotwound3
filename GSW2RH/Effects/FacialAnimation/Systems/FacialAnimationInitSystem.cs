using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.FacialAnimation.Systems
{
    [EcsInject]
    public class FacialAnimationInitSystem : BaseEffectInitSystem
    {
        public FacialAnimationInitSystem() : base(new GswLogger(typeof(FacialAnimationInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
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
                    if(string.IsNullOrEmpty(animation)) continue;
                    
                    component.Animations.Add(animation);
                }
            }

            XElement disable = partRoot.Element("DisableFacialAnimation");
            if (disable != null)
            {
                EcsWorld.AddComponent<DisableFacialAnimationComponent>(partEntity);
            }
        }
    }
}
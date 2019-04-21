using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.CameraShake.Systems
{
    [EcsInject]
    public class CameraShakeInitSystem : BaseEffectInitSystem
    {
        public CameraShakeInitSystem() : base(new GswLogger(typeof(CameraShakeInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement shake = partRoot.Element("CameraShake");
            if (shake != null)
            {
                var permanent = EcsWorld.AddComponent<EnablePermanentCameraShakeComponent>(partEntity);
                permanent.DisableOnlyOnHeal = shake.GetBool("DisableOnlyOnHeal");
                permanent.ShakeName = shake.GetAttributeValue("ShakeName");
                permanent.Intensity = shake.GetFloat("Intensity");
                permanent.Priority = shake.GetInt("Priority");
                permanent.PedAccuracy = shake.GetFloat("PedAccuracy");
                return;
            }

            XElement disable = partRoot.Element("DisableCameraShake");
            if (disable != null)
            {
                EcsWorld.AddComponent<DisableCameraShakeComponent>(partEntity);
            }
        }
    }
}
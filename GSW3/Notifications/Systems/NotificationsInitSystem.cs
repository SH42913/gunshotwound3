using Leopotam.Ecs;

namespace GSW3.Notifications.Systems
{
    [EcsInject]
    public class NotificationsInitSystem : IEcsPreInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        public void PreInitialize()
        {
            EcsEntity mainEntity = GunshotWound3.StatsContainerEntity;
            var settings = _ecsWorld.AddComponent<NotificationSettingsComponent>(mainEntity);
            settings.CombineToOne = true;

            _ecsWorld.CreateEntityWith(out NotificationComponent startNotification);
            startNotification.Message = "Great thanks for using ~g~GunShot Wound ~r~3~s~ by SH42913";
            startNotification.Delay = 20f;
        }

        public void PreDestroy()
        {
        }
    }
}
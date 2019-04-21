using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.Notifications.Systems
{
    [EcsInject]
    public class NotificationsSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly GameService _gameService = null;
        
        private readonly EcsFilter<NotificationSettingsComponent> _settings = null;
        private readonly EcsFilter<NotificationComponent> _notifications = null;
        
        public void Run()
        {
            if (_notifications.IsEmpty() || _gameService.GameIsPaused) return;
            
            float delta = GswExtensions.GetDeltaTime();
            NotificationSettingsComponent settings = _settings.Components1[0];
            if (settings.CombineToOne)
            {
                string finalMessage = "";
                foreach (int i in _notifications)
                {
                    NotificationComponent notification = _notifications.Components1[i];
                    notification.Delay -= delta;
                    if (notification.Delay <= 0)
                    {
                        finalMessage += notification.Message + "\n";
                        _ecsWorld.RemoveComponent<NotificationComponent>(_notifications.Entities[i]);
                    }
                }

                if (!string.IsNullOrEmpty(finalMessage))
                {
                    Game.DisplayNotification(finalMessage);
                }
            }
            else
            {
                foreach (int i in _notifications)
                {
                    NotificationComponent notification = _notifications.Components1[i];
                    notification.Delay -= delta;
                    if (notification.Delay <= 0)
                    {
                        if (!string.IsNullOrEmpty(notification.Message))
                        {
                            Game.DisplayNotification(notification.Message);
                        }
                        _ecsWorld.RemoveComponent<NotificationComponent>(_notifications.Entities[i]);
                    }
                }
            }
        }
    }
}
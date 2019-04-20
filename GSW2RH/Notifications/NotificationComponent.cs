using Leopotam.Ecs;

namespace GunshotWound2.Notifications
{
    public class NotificationComponent : IEcsAutoResetComponent
    {
        public string Message;
        public float Delay;

        public void Reset()
        {
            Message = null;
            Delay = -1f;
        }
    }
}
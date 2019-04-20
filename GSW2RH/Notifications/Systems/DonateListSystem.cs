using Leopotam.Ecs;

namespace GunshotWound2.Notifications.Systems
{
    [EcsInject]
    public class DonateListSystem : IEcsInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private const string LIST = "~g~GunShot Wound ~r~3 is supported by...\n" +
                                    "~y~My Little Stormtroopers from Patreon:\n" +
                                    "- Dawid Drzewiecki\n" +
                                    "~r~Other donates:\n" +
                                    "- Waterfall" +
                                    "~s~You can be one of supporters ;)";
        
        public void Initialize()
        {
            _ecsWorld.CreateEntityWith(out NotificationComponent notification);
            notification.Message = LIST;
            notification.Delay = 4f;
        }

        public void Destroy()
        {
        }
    }
}
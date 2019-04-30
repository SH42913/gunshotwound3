using Leopotam.Ecs;

namespace GSW3.Notifications.Systems
{
    [EcsInject]
    public class DonateListSystem : IEcsInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private const string DONATER_LIST = "~g~GunShot Wound ~r~3~s~ is supported by...\n" +
                                    "~y~My Little Stormtroopers from Patreon:\n" +
                                    "- Dawid Drzewiecki\n" +
                                    "- Jerry Schell\n" +
                                    "- Patrik Šarabok\n" +
                                    "- Curtis Lobley\n" +
                                    "~r~Other donaters:\n" +
                                    "- Waterfall\n" +
                                    "~s~You can be one of supporters ;)";
        
        public void Initialize()
        {
            _ecsWorld.CreateEntityWith(out NotificationComponent notification);
            notification.Message = DONATER_LIST;
            notification.Delay = 21f;
        }

        public void Destroy()
        {
        }
    }
}
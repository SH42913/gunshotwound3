using System.Globalization;
using System.Threading;
using Rage.Attributes;

[assembly: Plugin(
    "GunShot Wound 3",
    Description = "GunShot Wound 3 is bringing most realistic damage system to GTAV.",
    Author = "SH42913",
    ShouldTickInPauseMenu = true)]

namespace GSW3
{
    public static class Startup
    {
        private static readonly GunshotWound3 Gsw3Instance;
        
        static Startup()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            Gsw3Instance = new GunshotWound3();
            Gsw3Instance.Init();
        }
        
        public static void Main()
        {
            Gsw3Instance.IsRunning = true;
            Gsw3Instance.Run();
            Gsw3Instance.Dispose();
        }
    }
}
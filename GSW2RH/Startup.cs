using System.Globalization;
using System.Threading;

[assembly: Rage.Attributes.Plugin(
    "GunShot Wound 2",
    Description = "GunShot Wound 2 will bring most realistic damage system to GTAV.",
    Author = "SH42913")]

namespace GunshotWound2
{
    public static class Startup
    {
        public static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            var gsw2 = new GunshotWound2Script();
            gsw2.Init();
            gsw2.IsRunning = true;
            gsw2.Run();
            gsw2.Dispose();
        }
    }
}
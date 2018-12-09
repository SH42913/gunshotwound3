using System.Xml.Linq;
using Leopotam.Ecs;

namespace GunshotWound2.Weapons
{
    [EcsOneFrame]
    public class WeaponInitComponent : IEcsAutoResetComponent
    {
        public XElement WeaponRoot;

        public void Reset()
        {
            WeaponRoot = null;
        }
    }
}
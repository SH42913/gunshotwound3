using System.Xml.Linq;
using Leopotam.Ecs;

namespace GunshotWound2.Configs
{
    [EcsOneFrame]
    public class LoadedItemConfigComponent : IEcsAutoResetComponent
    {
        public XElement ElementRoot;

        public void Reset()
        {
            ElementRoot = null;
        }
    }
}
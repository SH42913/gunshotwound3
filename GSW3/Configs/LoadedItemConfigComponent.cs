using System.Xml.Linq;
using Leopotam.Ecs;

namespace GSW3.Configs
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
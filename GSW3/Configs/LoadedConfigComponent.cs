using System.Xml.Linq;
using Leopotam.Ecs;

namespace GSW3.Configs
{
    [EcsOneFrame]
    public class LoadedConfigComponent : IEcsAutoResetComponent
    {
        public string Path;
        public XElement ElementRoot;

        public void Reset()
        {
            Path = null;
            ElementRoot = null;
        }
    }
}
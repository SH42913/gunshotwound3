using System.Xml.Linq;
using Leopotam.Ecs;

namespace GunshotWound2.Configs
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
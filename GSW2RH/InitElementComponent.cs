using System.Xml.Linq;
using Leopotam.Ecs;

namespace GunshotWound2
{
    [EcsOneFrame]
    public class InitElementComponent : IEcsAutoResetComponent
    {
        public XElement ElementRoot;

        public void Reset()
        {
            ElementRoot = null;
        }
    }
}
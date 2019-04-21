using System.Collections.Generic;
using GSW3.WoundEffects.NaturalMotion.Arguments;

namespace GSW3.WoundEffects.NaturalMotion
{
    public class NaturalMotionMessage
    {
        public string Name;
        public List<NmArgument> NmArguments;
        public List<RandomFloatArgument> RandomFloatArguments;
        public List<RandomIntArgument> RandomIntArguments;
    }
}
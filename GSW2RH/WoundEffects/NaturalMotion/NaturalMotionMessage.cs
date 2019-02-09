using System.Collections.Generic;
using GunshotWound2.WoundEffects.NaturalMotion.Arguments;

namespace GunshotWound2.WoundEffects.NaturalMotion
{
    public class NaturalMotionMessage
    {
        public string Name;
        public List<NmArgument> NmArguments;
        public List<RandomFloatArgument> RandomFloatArguments;
        public List<RandomIntArgument> RandomIntArguments;
    }
}
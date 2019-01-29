using System.Collections.Generic;
using GunshotWound2.Effects.NaturalMotion.Arguments;

namespace GunshotWound2.Effects.NaturalMotion
{
    public class NaturalMotionMessage
    {
        public string Name;
        public List<NmArgument> NmArguments;
        public List<RandomFloatArgument> RandomFloatArguments;
        public List<RandomIntArgument> RandomIntArguments;
    }
}
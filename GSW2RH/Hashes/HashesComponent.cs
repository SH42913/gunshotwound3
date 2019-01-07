using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Hashes
{
    public class HashesComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck] 
        public readonly List<uint> Hashes = new List<uint>();

        public void Reset()
        {
            Hashes.Clear();
        }

        public override string ToString()
        {
            string hashes = "Hashes: ";
            if (Hashes == null || Hashes.Count <= 0)
            {
                hashes += "nothing to see here";
                return hashes;
            }

            foreach (uint hash in Hashes)
            {
                hashes += hash + "; ";
            }

            return hashes;
        }
    }
}
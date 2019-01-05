using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2
{
    public class HashesComponent : IEcsAutoResetComponent
    {
        public string Name;

        [EcsIgnoreNullCheck] 
        public readonly List<uint> Hashes = new List<uint>();

        public void Reset()
        {
            Name = null;
            Hashes.Clear();
        }

        public override string ToString()
        {
            string hashes = Name + " hashes: ";
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
using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Weapons
{
    public class WeaponHashesComponent : IEcsAutoResetComponent
    {
        public string Name;
        public List<uint> Hashes;

        public void Reset()
        {
            Name = null;
            Hashes = null;
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
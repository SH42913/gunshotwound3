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
    }
}
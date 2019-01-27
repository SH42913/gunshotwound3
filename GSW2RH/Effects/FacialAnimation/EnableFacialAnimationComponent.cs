using System.Collections.Generic;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.FacialAnimation
{
    public class EnableFacialAnimationComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly List<string> Animations = new List<string>();

        public string MaleDict;
        public string FemaleDict;
        public bool Permanent;

        public void Reset()
        {
            Animations.Clear();
        }
    }
}
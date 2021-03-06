using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.WoundEffects.FacialAnimation
{
    public class EnableFacialAnimationComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck] public readonly List<string> Animations = new List<string>(8);

        public string MaleDict;
        public string FemaleDict;
        public bool Permanent;

        public void Reset()
        {
            Animations.Clear();
        }

        public override string ToString()
        {
            string animationList = "";
            foreach (string animation in Animations)
            {
                animationList += animation + ";";
            }

            return nameof(EnableFacialAnimationComponent) + ": " +
                   nameof(MaleDict) + " " + MaleDict + "; " +
                   nameof(FemaleDict) + " " + FemaleDict + "; " +
                   nameof(Permanent) + " " + Permanent + "; " +
                   nameof(Animations) + " " + animationList + "; ";
        }
    }
}
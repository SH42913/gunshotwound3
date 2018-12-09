﻿using Leopotam.Ecs;

namespace GunshotWound2.HitDetecting.Components
{
    public enum BodyParts
    {
        HEAD,
        NECK,
        UPPER_BODY,
        LOWER_BODY,
        ARM,
        LEG,
        NOTHING
    }
    
    [EcsOneFrame]
    public class DamagedBodyPartComponent
    {
        public BodyParts DamagedBodyPart;
    }
}
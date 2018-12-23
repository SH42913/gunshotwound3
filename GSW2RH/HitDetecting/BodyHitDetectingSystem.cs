﻿using System;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.HitDetecting
{
    [EcsInject]
    public class BodyHitDetectingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, HasBeenHitMarkComponent> _damagedPeds;

        private readonly GswLogger _logger;
        private static readonly Random Random = new Random();

        public BodyHitDetectingSystem()
        {
            _logger = new GswLogger(typeof(BodyHitDetectingSystem));
        }

        public void Run()
        {
            foreach (int i in _damagedPeds)
            {
                Ped ped = _damagedPeds.Components1[i].ThisPed;
                if (!ped.Exists()) continue;

                int pedEntity = _damagedPeds.Entities[i];
                BodyParts bodyPart = GetDamagedBodyPart(ped);
                if (bodyPart == BodyParts.NOTHING) continue;

#if DEBUG
                PedBoneId lastBone = ped.LastDamageBone;
                _logger.MakeLog($"Ped {ped.Name(pedEntity)} has damaged {bodyPart} with boneId {lastBone}");

                var history = _ecsWorld.EnsureComponent<BodyHitHistoryComponent>(pedEntity, out bool newBodyHitHistory);
                PedBoneId?[] bones = history.LastDamagedBones;
                if (newBodyHitHistory)
                {
                    bones[0] = lastBone;
                    bones[1] = null;
                    bones[2] = null;
                }
                else
                {
                    bones[2] = bones[1];
                    bones[1] = bones[0];
                    bones[2] = lastBone;
                }
#endif

                _ecsWorld.AddComponent<DamagedBodyPartComponent>(pedEntity).DamagedBodyPart = bodyPart;
                ped.ClearLastDamageBone();
            }
        }

        private BodyParts GetDamagedBodyPart(Ped target)
        {
            if (target == null)
            {
                return BodyParts.NOTHING;
            }

            PedBoneId lastBone = target.LastDamageBone;
            if (lastBone == 0)
            {
                _logger.MakeLog($"Ped {target.Model.Name} doesn\'t have damaged bone!");
                return Random.NextEnum<BodyParts>();
            }

            switch (lastBone)
            {
                case PedBoneId.Head:
                    return BodyParts.HEAD;
                case PedBoneId.Neck:
                    return BodyParts.NECK;
                case PedBoneId.LeftClavicle:
                case PedBoneId.RightClavicle:
                case PedBoneId.Spine2:
                case PedBoneId.Spine3:
                    return BodyParts.UPPER_BODY;
                case PedBoneId.SpineRoot:
                case PedBoneId.Spine:
                case PedBoneId.Spine1:
                case PedBoneId.Pelvis:
                    return BodyParts.LOWER_BODY;
                case PedBoneId.LeftThigh:
                case PedBoneId.RightThigh:
                case PedBoneId.RightFoot:
                case PedBoneId.LeftFoot:
                case PedBoneId.RightPhFoot:
                case PedBoneId.LeftPhFoot:
                case PedBoneId.LeftCalf:
                case PedBoneId.RightCalf:
                    return BodyParts.LEG;
                case PedBoneId.LeftUpperArm:
                case PedBoneId.RightUpperArm:
                case PedBoneId.LeftForeArm:
                case PedBoneId.RightForearm:
                case PedBoneId.LeftHand:
                case PedBoneId.RightHand:
                case PedBoneId.LeftPhHand:
                case PedBoneId.RightPhHand:
                case PedBoneId.LeftThumb0:
                case PedBoneId.LeftThumb1:
                case PedBoneId.LeftThumb2:
                case PedBoneId.LeftIndexFinger0:
                case PedBoneId.LeftIndexFinger1:
                case PedBoneId.LeftIndexFinger2:
                case PedBoneId.LeftMiddleFinger0:
                case PedBoneId.LeftMiddleFinger1:
                case PedBoneId.LeftMiddleFinger2:
                case PedBoneId.LeftRingFinger0:
                case PedBoneId.LeftRingFinger1:
                case PedBoneId.LeftRingFinger2:
                case PedBoneId.LeftPinky0:
                case PedBoneId.LeftPinky1:
                case PedBoneId.LeftPinky2:
                case PedBoneId.RightThumb0:
                case PedBoneId.RightThumb1:
                case PedBoneId.RightThumb2:
                case PedBoneId.RightIndexFinger0:
                case PedBoneId.RightIndexFinger1:
                case PedBoneId.RightIndexFinger2:
                case PedBoneId.RightMiddleFinger0:
                case PedBoneId.RightMiddleFinger1:
                case PedBoneId.RightMiddleFinger2:
                case PedBoneId.RightRingFinger0:
                case PedBoneId.RightRingFinger1:
                case PedBoneId.RightRingFinger2:
                case PedBoneId.RightPinky0:
                case PedBoneId.RightPinky1:
                case PedBoneId.RightPinky2:
                    return BodyParts.ARM;
                default:
                    _logger.MakeLog($"Bone {target.LastDamageBone} for ped {target.Model.Name} is unknown!");
                    return Random.NextEnum<BodyParts>();
            }
        }
    }
}
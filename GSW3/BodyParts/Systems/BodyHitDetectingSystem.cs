using System;
using System.Collections.Generic;
using GSW3.BaseHitDetecting;
using GSW3.DebugSystems.DamagedBonesHistory;
using GSW3.GswWorld;
using GSW3.Utils;
using Leopotam.Ecs;
using Rage;

namespace GSW3.BodyParts.Systems
{
    [EcsInject]
    public class BodyHitDetectingSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;
        private readonly Random _random = null;

        private readonly EcsFilter<GswPedComponent, HasBeenHitMarkComponent> _damagedPeds = null;
        private readonly EcsFilter<BodyPartComponent> _bodyParts = null;
        private readonly EcsFilter<BoneToBodyPartDictComponent> _bodyPartList = null;

        private readonly GswLogger _logger;

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

                EcsEntity pedEntity = _damagedPeds.Entities[i];
                PedBoneId lastBone = ped.LastDamageBone;
                EcsEntity bodyPartEntity = GetDamagedBodyPart(ped);

#if DEBUG
                string partName = bodyPartEntity.GetEntityName();
                _logger.MakeLog($"{pedEntity.GetEntityName()} has damaged {partName} with boneId {(uint) lastBone}");

                var history = _ecsWorld.EnsureComponent<DamagedBoneHistoryComponent>(pedEntity, out bool isNew);
                PedBoneId?[] bones = history.LastDamagedBones;
                if (isNew)
                {
                    bones[0] = lastBone;
                    bones[1] = null;
                    bones[2] = null;
                }
                else
                {
                    bones[2] = bones[1];
                    bones[1] = bones[0];
                    bones[0] = lastBone;
                }
#endif

                var damagedBodyPart = _ecsWorld.AddComponent<DamagedBodyPartComponent>(pedEntity);
                damagedBodyPart.DamagedBodyPartEntity = bodyPartEntity;
                damagedBodyPart.DamagedBoneId = (uint) lastBone;
                ped.ClearLastDamageBone();
            }
        }

        private EcsEntity GetDamagedBodyPart(Ped target)
        {
            var lastBone = (uint) target.LastDamageBone;
            Dictionary<uint, EcsEntity> bodyPartDict = _bodyPartList.Components1[0].BoneIdToBodyPartEntity;
            if (bodyPartDict.TryGetValue(lastBone, out EcsEntity damagedBodyPartEntity))
            {
                return damagedBodyPartEntity;
            }

            _logger.MakeLog($"!!!WARNING!!! Bone {lastBone} for ped {target.Model.Name} is unknown! " +
                            "Bone will be selected by random!");
            return GetRandomBodyPartEntity();
        }

        private EcsEntity GetRandomBodyPartEntity()
        {
            return _bodyParts.Entities[_random.Next(0, _bodyParts.GetEnumerator().GetCount())];
        }
    }
}
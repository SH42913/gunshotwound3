using System;
using System.Collections.Generic;
using GunshotWound2.BaseHitDetecting;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

#if DEBUG
using GunshotWound2.DebugSystems.DamagedBonesHistory;
#endif

namespace GunshotWound2.BodyParts.Systems
{
    [EcsInject]
    public class BodyHitDetectingSystem : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<GswPedComponent, HasBeenHitMarkComponent> _damagedPeds = null;
        private readonly EcsFilter<BodyPartComponent> _bodyParts = null;
        private readonly EcsFilter<BoneToBodyPartDictComponent> _bodyPartList = null;

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
                int bodyPartEntity = GetDamagedBodyPart(ped);
                if (bodyPartEntity < 0) continue;

#if DEBUG
                PedBoneId lastBone = ped.LastDamageBone;
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
                    bones[2] = lastBone;
                }
#endif

                _ecsWorld.AddComponent<DamagedBodyPartComponent>(pedEntity).DamagedBodyPartEntity = bodyPartEntity;
                ped.ClearLastDamageBone();
            }
        }

        private int GetDamagedBodyPart(Ped target)
        {
            var lastBone = (uint) target.LastDamageBone;
            Dictionary<uint, int> bodyPartDict = _bodyPartList.Components1[0].BoneIdToBodyPartEntity;
            if (bodyPartDict.ContainsKey(lastBone))
            {
                return bodyPartDict[lastBone];
            }

            _logger.MakeLog($"WARNING! Bone {lastBone} for ped {target.Model.Name} is unknown! " +
                            "Bone will select by random!");
            return GetRandomBodyPartEntity();
        }

        private int GetRandomBodyPartEntity()
        {
            return _bodyParts.Entities[Random.Next(0, _bodyParts.GetEnumerator().GetCount())];
        }
    }
}
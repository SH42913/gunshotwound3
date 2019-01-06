using System;
using GunshotWound2.BaseHitDetecting;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Bodies.Systems
{
    [EcsInject]
    public class BodyHitDetectingSystem : IEcsRunSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<GswPedComponent, HasBeenHitMarkComponent> _damagedPeds;
        private EcsFilter<BodyPartComponent> _bodyParts;
        private EcsFilter<BodyPartListComponent> _bodyPartList;

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
                string bodyPartName = _ecsWorld.GetComponent<HashesComponent>(bodyPartEntity).Name;
                _logger.MakeLog($"Ped {ped.Name(pedEntity)} has damaged {bodyPartName} with boneId {(uint) lastBone}");

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

                _ecsWorld.AddComponent<DamagedBodyPartComponent>(pedEntity).DamagedBodyPartEntity = bodyPartEntity;
                ped.ClearLastDamageBone();
            }
        }

        private int GetDamagedBodyPart(Ped target)
        {
            var lastBone = (uint) target.LastDamageBone;
            if (lastBone == 0)
            {
                _logger.MakeLog($"!!! Ped {target.Model.Name} doesn\'t have damaged bone! Bone was select by random!");
                return GetRandomBodyPartEntity();
            }

            var bodyPartDict = _bodyPartList.Components1[0].BoneIdToBodyPartEntity;
            if (bodyPartDict.ContainsKey(lastBone))
            {
                return bodyPartDict[lastBone];
            }

            _logger.MakeLog($"!!! Bone {lastBone} for ped {target.Model.Name} is unknown!");
            return GetRandomBodyPartEntity();
        }

        private int GetRandomBodyPartEntity()
        {
            return _bodyParts.Entities[Random.Next(0, _bodyParts.EntitiesCount)];
        }
    }
}
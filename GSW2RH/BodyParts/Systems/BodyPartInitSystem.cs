using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Hashes;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.BodyParts.Systems
{
    [EcsInject]
    public class BodyPartInitSystem : IEcsPreInitSystem, IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedConfigComponent> _loadedConfigs;
        private EcsFilter<HashesComponent, BodyPartComponent> _partsWithHashes;

        private readonly GswLogger _logger;
        private const string BODY_PART_LIST = "BodyPartList";

        public BodyPartInitSystem()
        {
            _logger = new GswLogger(typeof(BodyPartInitSystem));
        }

        public void PreInitialize()
        {
            _logger.MakeLog("BodyPart list is loading!");

            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;

                var listElement = xmlRoot.Element(BODY_PART_LIST);
                if (listElement == null) continue;
                
                foreach (XElement bodyPartRoot in listElement.Elements("BodyPart"))
                {
                    CreateBodyPart(bodyPartRoot);
                }
            }

            _logger.MakeLog("BodyPart list loaded!");
        }

        private void CreateBodyPart(XElement bodyPartRoot)
        {
            _ecsWorld.CreateEntityWith(out BodyPartComponent _, out LoadedItemConfigComponent initComponent);
            initComponent.ElementRoot = bodyPartRoot;
        }

        public void Initialize()
        {
            var listComponent = _ecsWorld.CreateEntityWith<BoneToBodyPartDictComponent>();

            foreach (int i in _partsWithHashes)
            {
                HashesComponent hashesComponent = _partsWithHashes.Components1[i];
                int entity = _partsWithHashes.Entities[i];
                
                foreach (uint hash in hashesComponent.Hashes)
                {
                    listComponent.BoneIdToBodyPartEntity.Add(hash, entity);
                }
            }
        }

        public void PreDestroy()
        {
        }

        public void Destroy()
        {
        }
    }
}
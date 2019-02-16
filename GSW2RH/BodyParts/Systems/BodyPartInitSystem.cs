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
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;
        private readonly EcsFilter<HashesComponent, BodyPartComponent> _partsWithHashes = null;

        private readonly GswLogger _logger;

        public BodyPartInitSystem()
        {
            _logger = new GswLogger(typeof(BodyPartInitSystem));
        }

        public void PreInitialize()
        {
            foreach (int i in _loadedConfigs)
            {
                LoadedConfigComponent config = _loadedConfigs.Components1[i];
                XElement xmlRoot = config.ElementRoot;
                XElement listElement = xmlRoot.Element("BodyPartList");
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
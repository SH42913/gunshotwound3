using System.Xml.Linq;
using GSW3.Configs;
using GSW3.Hashes;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.BodyParts.Systems
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

            _logger.MakeLog("BodyPartList loaded!");
        }

        private void CreateBodyPart(XElement bodyPartRoot)
        {
            _ecsWorld.CreateEntityWith(out BodyPartComponent _, out LoadedItemConfigComponent initComponent);
            initComponent.ElementRoot = bodyPartRoot;
        }

        public void Initialize()
        {
            _ecsWorld.CreateEntityWith(out BoneToBodyPartDictComponent dictComponent);
            foreach (int i in _partsWithHashes)
            {
                HashesComponent hashesComponent = _partsWithHashes.Components1[i];
                EcsEntity entity = _partsWithHashes.Entities[i];
                foreach (uint hash in hashesComponent.Hashes)
                {
                    dictComponent.BoneIdToBodyPartEntity.Add(hash, entity);
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
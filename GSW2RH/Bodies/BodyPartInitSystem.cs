using System;
using System.IO;
using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Bodies
{
    [EcsInject]
    public class BodyPartInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;

        private readonly GswLogger _logger;

        public BodyPartInitSystem()
        {
            _logger = new GswLogger(typeof(BodyPartInitSystem));
        }

        public void PreInitialize()
        {
            try
            {
                GenerateBodyParts();
            }
            catch (Exception e)
            {
                _logger.MakeLog(e.Message);
            }
        }

        private void GenerateBodyParts()
        {
            string fullPath = Environment.CurrentDirectory + GunshotWound2Script.WOUND_CONFIG_PATH;
            var file = new FileInfo(fullPath);
            if (!file.Exists)
            {
                throw new Exception($"Can\'t find {fullPath}");
            }

            XElement xmlRoot = XDocument.Load(file.OpenRead()).Root;
            if (xmlRoot == null)
            {
                throw new Exception($"Can\'t find root in {fullPath}");
            }

            XElement bodyPartList = xmlRoot.Element("BodyPartList");
            if (bodyPartList == null)
            {
                throw new Exception($"Can\'t find BodyPartList in {fullPath}");
            }

            var listComponent = _ecsWorld.CreateEntityWith<BodyPartListComponent>();
            foreach (XElement bodyPart in bodyPartList.Elements("BodyPart"))
            {
                CreateBodyPart(bodyPart, listComponent);
            }
        }

        private void CreateBodyPart(XElement bodyPartRoot, BodyPartListComponent listComponent)
        {
            XElement hashesElement = bodyPartRoot.GetElement("BoneHashes");

            int entity = _ecsWorld.CreateEntityWith(out BodyPartComponent _, out InitElementComponent initComponent);
            initComponent.ElementRoot = bodyPartRoot;

            var hashesComponent = _ecsWorld.AddComponent<HashesComponent>(entity);
            hashesComponent.FillHashesComponent(hashesElement, _logger);

            foreach (uint hash in hashesComponent.Hashes)
            {
                listComponent.BoneIdToBodyPartEntity.Add(hash, entity);
            }
        }

        public void PreDestroy()
        {
        }
    }
}
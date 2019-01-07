using System;
using System.IO;
using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Configs
{
    [EcsInject]
    public class ConfigInitSystem : IEcsPreInitSystem
    {
        private EcsWorld _ecsWorld;

        private readonly GswLogger _logger;

        private const string DATA_FOLDER_PATH = "\\Plugins\\GswData\\";

        public ConfigInitSystem()
        {
            _logger = new GswLogger(typeof(ConfigInitSystem));
        }
        
        public void PreInitialize()
        {
            foreach (string path in GunshotWound2Script.CONFIG_NAMES)
            {
                string fullPath = Environment.CurrentDirectory + DATA_FOLDER_PATH + path;
                var file = new FileInfo(fullPath);
                if (!file.Exists)
                {
                    throw new Exception($"Can\'t find {fullPath}");
                }

                XElement xmlRoot = XDocument.Load(file.OpenRead()).Root;
                if (xmlRoot == null)
                {
                    throw new Exception($"Can\'t find XML-root in {fullPath}");
                }

                var loadedConfig = _ecsWorld.CreateEntityWith<LoadedConfigComponent>();
                loadedConfig.Path = fullPath;
                loadedConfig.ElementRoot = xmlRoot;
            }
        }

        public void PreDestroy()
        {
        }
    }
}
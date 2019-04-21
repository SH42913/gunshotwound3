using System;
using System.IO;
using System.Xml.Linq;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.Configs.Systems
{
    [EcsInject]
    public class ConfigInitSystem : IEcsPreInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private const string BASE_CONFIG_NAME = "GswBaseConfig.xml";
        private const string DATA_FOLDER_PATH = "\\Plugins\\GswData\\";

        private readonly GswLogger _logger;

        public ConfigInitSystem()
        {
            _logger = new GswLogger(typeof(ConfigInitSystem));
        }

        public void PreInitialize()
        {
            XElement baseRoot = LoadConfig(BASE_CONFIG_NAME);
            XElement configList = baseRoot.GetElement("ConfigFiles");
            foreach (XElement config in configList.Elements("Config"))
            {
                string configPath = config.GetAttributeValue("Path");
                LoadConfig(configPath);
            }
        }

        public void PreDestroy()
        {
        }

        private XElement LoadConfig(string path)
        {
            string fullPath = Environment.CurrentDirectory + DATA_FOLDER_PATH + path;
            var file = new FileInfo(fullPath);
            if (!file.Exists)
            {
                throw new Exception($"Can't find {fullPath}");
            }

            XElement xmlRoot = XDocument.Load(file.OpenRead()).Root;
            if (xmlRoot == null)
            {
                throw new Exception($"Can't find XML-root in {fullPath}");
            }

            _ecsWorld.CreateEntityWith(out LoadedConfigComponent loadedConfig);
            loadedConfig.Path = fullPath;
            loadedConfig.ElementRoot = xmlRoot;
            _logger.MakeLog($"Loaded config {fullPath}");

            return xmlRoot;
        }
    }
}
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Hashes.Systems
{
    [EcsInject]
    public class HashesInitSystem : IEcsInitSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<LoadedItemConfigComponent> _configParts = null;

        private readonly GswLogger _logger;
        private const string HASHES = "Hashes";

        public HashesInitSystem()
        {
            _logger = new GswLogger(typeof(HashesInitSystem));
        }

        public void Initialize()
        {
            foreach (int i in _configParts)
            {
                XElement root = _configParts.Components1[i].ElementRoot;
                XElement hashesElement = root.Element(HASHES);
                if (hashesElement == null) continue;

                int entity = _configParts.Entities[i];
                var hashesComponent = _ecsWorld.AddComponent<HashesComponent>(entity);

                string[] hashStrings = hashesElement.GetAttributeValue("Hashes").Split(';');
                foreach (string hashString in hashStrings)
                {
                    if (string.IsNullOrEmpty(hashString)) continue;

                    if (uint.TryParse(hashString, out uint hash))
                    {
                        hashesComponent.Hashes.Add(hash);
                    }
                    else
                    {
                        _logger.MakeLog($"WARNING! Wrong hash: {hashString}");
                    }
                }

#if DEBUG
                _logger.MakeLog($"{entity.GetEntityName()} have got {hashesComponent}");
#endif
            }
        }

        public void Destroy()
        {
        }
    }
}
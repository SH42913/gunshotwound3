using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionInitSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private EcsFilter<LoadedItemConfigComponent> _configParts;

        private readonly GswLogger _logger;

        public NaturalMotionInitSystem()
        {
            _logger = new GswLogger(typeof(NaturalMotionInitSystem));
        }

        public void Initialize()
        {
            foreach (int i in _configParts)
            {
                XElement root = _configParts.Components1[i].ElementRoot;
                XElement listElement = root.Element("NaturalMotionMessages");
                if (listElement == null) continue;

                int entity = _configParts.Entities[i];
                var messages = _ecsWorld.AddComponent<NaturalMotionMessagesComponent>(entity);

                var messageStrings = listElement.GetAttributeValue("List").Split(';');
                foreach (string messageString in messageStrings)
                {
                    if (string.IsNullOrEmpty(messageString)) continue;

                    if (int.TryParse(messageString, out int message) && message > 0 && message < 1350)
                    {
                        messages.MessageList.Add(message);
                    }
                    else
                    {
                        _logger.MakeLog($"Wrong NaturalMotionMessage: {messageString}");
                    }
                }

                _logger.MakeLog($"{entity.GetEntityName(_ecsWorld)} {messages}");
            }
        }

        public void Destroy()
        {
        }
    }
}
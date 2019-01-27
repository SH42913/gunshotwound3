using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionInitSystem : BaseEffectInitSystem
    {
        public NaturalMotionInitSystem() : base(new GswLogger(typeof(NaturalMotionInitSystem)))
        {
        }

        protected override void CheckPart(XElement partRoot, int partEntity)
        {
            XElement listElement = partRoot.Element("NaturalMotionMessages");
            if (listElement == null) return;

            var messages = EcsWorld.AddComponent<NaturalMotionMessagesComponent>(partEntity);

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
                    Logger.MakeLog($"Wrong NaturalMotionMessage: {messageString}");
                }
            }

            Logger.MakeLog($"{partEntity.GetEntityName(EcsWorld)} {messages}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using GunshotWound2.WoundEffects.NaturalMotion.Arguments;
using Leopotam.Ecs;

namespace GunshotWound2.WoundEffects.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionInitSystem : BaseEffectInitSystem, IEcsPreInitSystem
    {
        private readonly EcsFilter<LoadedConfigComponent> _loadedConfig = null;
        private readonly EcsFilter<NaturalMotionMessagesDictComponent> _dict = null;

        public NaturalMotionInitSystem() : base(new GswLogger(typeof(NaturalMotionInitSystem)))
        {
        }

        public void PreInitialize()
        {
            EcsEntity statsEntity = GunshotWound2Script.StatsContainerEntity;
            var dict = EcsWorld.AddComponent<NaturalMotionMessagesDictComponent>(statsEntity);
            foreach (int i in _loadedConfig)
            {
                XElement root = _loadedConfig.Components1[i].ElementRoot;
                XElement list = root.Element("NaturalMotionMessageList");
                if (list == null) continue;

                foreach (XElement element in list.Elements("NaturalMotionMessage"))
                {
                    var message = new NaturalMotionMessage
                    {
                        Name = element.Element("MessageName").GetAttributeValue("Value"),
                        NmArguments = new List<NmArgument>(),
                        RandomFloatArguments = new List<RandomFloatArgument>(),
                        RandomIntArguments = new List<RandomIntArgument>()
                    };

                    foreach (XElement boolElement in element.Elements("BoolArgument"))
                    {
                        message.NmArguments.Add(new NmArgument
                        {
                            Name = boolElement.GetAttributeValue("Name"),
                            Value = boolElement.GetBool()
                        });
                    }

                    foreach (XElement intElement in element.Elements("IntArgument"))
                    {
                        message.NmArguments.Add(new NmArgument
                        {
                            Name = intElement.GetAttributeValue("Name"),
                            Value = intElement.GetInt()
                        });
                    }

                    foreach (XElement floatElement in element.Elements("FloatArgument"))
                    {
                        message.NmArguments.Add(new NmArgument
                        {
                            Name = floatElement.GetAttributeValue("Name"),
                            Value = floatElement.GetFloat()
                        });
                    }

                    foreach (XElement randomFloat in element.Elements("RandomFloatArgument"))
                    {
                        message.RandomFloatArguments.Add(new RandomFloatArgument
                        {
                            Name = randomFloat.GetAttributeValue("Name"),
                            Value = randomFloat.GetMinMax()
                        });
                    }

                    foreach (XElement randomFloat in element.Elements("RandomIntArgument"))
                    {
                        message.RandomIntArguments.Add(new RandomIntArgument
                        {
                            Name = randomFloat.GetAttributeValue("Name"),
                            Value = randomFloat.GetMinMaxInt()
                        });
                    }

                    dict.MessageDict.Add(element.GetAttributeValue("Name"), message);
                }
            }
        }

        protected override void CheckPart(XElement partRoot, EcsEntity partEntity)
        {
            XElement listElement = partRoot.Element("NaturalMotionMessages");
            if (listElement == null) return;

            Dictionary<string, NaturalMotionMessage> dict = _dict.Components1[0].MessageDict;
            var messages = EcsWorld.AddComponent<NaturalMotionMessagesComponent>(partEntity);
            string[] messageStrings = listElement.GetAttributeValue("List").Split(';');
            foreach (string messageString in messageStrings)
            {
                if (string.IsNullOrEmpty(messageString)) continue;

                if (!dict.ContainsKey(messageString))
                {
                    throw new Exception($"NaturalMotionMessage {messageString} doesn't exist!");
                }

                messages.MessageList.Add(messageString);
            }

            messages.PlayInPermanentRagdoll = listElement.GetBool("PlayInPermanentRagdoll");
            Logger.MakeLog($"{partEntity.GetEntityName()} have got {messages}");
        }

        public void PreDestroy()
        {
        }
    }
}
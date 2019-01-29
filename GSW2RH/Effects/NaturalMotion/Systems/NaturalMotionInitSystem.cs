using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Effects.NaturalMotion.Arguments;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Effects.NaturalMotion.Systems
{
    [EcsInject]
    public class NaturalMotionInitSystem : BaseEffectInitSystem, IEcsPreInitSystem
    {
        private EcsFilter<LoadedConfigComponent> _loadedConfig;
        private EcsFilter<NaturalMotionMessagesDictComponent> _dict;

        public NaturalMotionInitSystem() : base(new GswLogger(typeof(NaturalMotionInitSystem)))
        {
        }

        public void PreInitialize()
        {
            var dict = EcsWorld.AddComponent<NaturalMotionMessagesDictComponent>(GunshotWound2Script.StatsContainerEntity);

            foreach (int i in _loadedConfig)
            {
                XElement root = _loadedConfig.Components1[i].ElementRoot;

                var list = root.Element("NaturalMotionMessageList");
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

        protected override void CheckPart(XElement partRoot, int partEntity)
        {
            XElement listElement = partRoot.Element("NaturalMotionMessages");
            if (listElement == null) return;

            var messages = EcsWorld.AddComponent<NaturalMotionMessagesComponent>(partEntity);
            var messageStrings = listElement.GetAttributeValue("List").Split(';');
            foreach (string messageString in messageStrings)
            {
                if (string.IsNullOrEmpty(messageString)) continue;

                if (!_dict.Components1[0].MessageDict.ContainsKey(messageString))
                {
                    throw new Exception($"NaturalMotionMessage {messageString} doesn't exist!");
                }

                messages.MessageList.Add(messageString);
            }

            Logger.MakeLog($"{partEntity.GetEntityName(EcsWorld)} have {messages}");
        }

        public void PreDestroy()
        {
        }
    }
}
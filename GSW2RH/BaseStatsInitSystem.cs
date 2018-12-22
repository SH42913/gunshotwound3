using System;
using System.IO;
using System.Xml.Linq;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.WoundProcessing
{
    [EcsInject]
    public abstract class BaseStatsInitSystem<T> : IEcsPreInitSystem where T : class, new()
    {
        protected EcsWorld EcsWorld;
        
        protected abstract string ConfigPath { get; }
        protected abstract GswLogger Logger { get; }
        
        public void PreInitialize()
        {
            var stats = EcsWorld.AddComponent<T>(GunshotWound2Script.StatsContainerEntity);
            FillWithDefaultValues(stats);
            
            try
            {
                string fullPath = Environment.CurrentDirectory + ConfigPath;
                var file = new FileInfo(fullPath);
                if (!file.Exists)
                {
                    throw new Exception($"Can\'t find {fullPath}");
                }

                XElement xmlRoot = XDocument.Load(file.OpenRead()).Root;
                if (xmlRoot == null)
                {
                    throw new Exception($"Can\'t find root in {ConfigPath}");
                }
                
                FillWithConfigValues(stats, xmlRoot);
            }
            catch (Exception e)
            {
                Logger.MakeLog(e.Message);
                FillWithDefaultValues(stats);
            }
            
#if DEBUG
            Logger.MakeLog(stats.ToString());
#endif
        }

        protected abstract void FillWithDefaultValues(T stats);
        protected abstract void FillWithConfigValues(T stats, XElement xmlRoot);

        public void PreDestroy()
        {
        }
    }
}
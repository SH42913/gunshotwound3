using System.Xml.Linq;
using GSW3.Configs;
using GSW3.Utils;
using Leopotam.Ecs;

namespace GSW3.WoundEffects
{
    [EcsInject]
    public abstract class BaseEffectInitSystem : IEcsInitSystem
    {
        protected EcsWorld EcsWorld;
        protected EcsFilter<LoadedItemConfigComponent> InitParts;

        protected readonly GswLogger Logger;

        protected BaseEffectInitSystem(GswLogger logger)
        {
            Logger = logger;
        }

        public void Initialize()
        {
            foreach (int i in InitParts)
            {
                XElement partRoot = InitParts.Components1[i].ElementRoot;
                EcsEntity partEntity = InitParts.Entities[i];

                CheckPart(partRoot, partEntity);
            }
        }

        protected abstract void CheckPart(XElement partRoot, EcsEntity partEntity);

        public void Destroy()
        {
        }
    }
}
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.Utils;
using Leopotam.Ecs;

namespace GunshotWound2.Effects
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
                int partEntity = InitParts.Entities[i];

                CheckPart(partRoot, partEntity);
            }
        }

        protected abstract void CheckPart(XElement partRoot, int partEntity);

        public void Destroy()
        {
        }
    }
}
using System;
using System.Xml.Linq;
using GunshotWound2.GswWorld;
using GunshotWound2.Health;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;

namespace GunshotWound2.Pain.Systems
{
    public class PedPainInitSystem : BaseStatsInitSystem<PedPainStatsComponent>, IEcsRunSystem
    {
        protected override string ConfigPath { get; }
        protected override GswLogger Logger { get; }

        private EcsFilter<PedPainStatsComponent> _painStats;
        private EcsFilter<PedHealthStatsComponent> _healthStats;
        private EcsFilter<NewPedMarkComponent>.Exclude<AnimalMarkComponent> _newHumans;
        private EcsFilter<GswPedComponent, NewPedMarkComponent, AnimalMarkComponent> _newAnimals;
        private static readonly Random Random = new Random();

        public PedPainInitSystem()
        {
            ConfigPath = GunshotWound2Script.WORLD_CONFIG_PATH;
            Logger = new GswLogger(typeof(PedPainInitSystem));
        }
        
        protected override void FillWithDefaultValues(PedPainStatsComponent stats)
        {
            stats.PedUnbearablePain = new MinMax
            {
                Min = 50,
                Max = 100
            };
            stats.PedPainRecoverySpeed = new MinMax
            {
                Min = 0.5f,
                Max = 1f
            };
        }

        protected override void FillWithConfigValues(PedPainStatsComponent stats, XElement xmlRoot)
        {
            XElement pedUnbearablePain = xmlRoot.GetElement("PedUnbearablePain");
            XElement pedPainRecoverySpeed = xmlRoot.GetElement("PedPainRecoverySpeed");
            
            var painLimit = pedUnbearablePain.GetMinMax();
            if (!painLimit.IsDisabled())
            {
                stats.PedUnbearablePain = painLimit;
            }
            
            var recoverySpeed = pedPainRecoverySpeed.GetMinMax();
            if (!recoverySpeed.IsDisabled())
            {
                stats.PedPainRecoverySpeed = recoverySpeed;
            }
        }
        
        public void Run()
        {
            if(_painStats.EntitiesCount <= 0) return;
            PedPainStatsComponent stats = _painStats.Components1[0];
            
            foreach (int i in _newHumans)
            {
                int humanEntity = _newHumans.Entities[i];

                var painInfo = EcsWorld.AddComponent<PainInfoComponent>(humanEntity);
                painInfo.UnbearablePain = Random.NextMinMax(stats.PedUnbearablePain);
                painInfo.PainRecoverySpeed = Random.NextMinMax(stats.PedPainRecoverySpeed);
                painInfo.CurrentPainState = PainStates.NONE;
            }
            
            if(_healthStats.EntitiesCount <= 0) return;
            PedHealthStatsComponent healthStats = _healthStats.Components1[0];
            
            foreach (int i in _newAnimals)
            {
                Ped ped = _newAnimals.Components1[i].ThisPed;
                int animalEntity = _newAnimals.Entities[i];

                float healthPercent = ped.GetHealth() / healthStats.PedHealth.Max;
                var painInfo = EcsWorld.AddComponent<PainInfoComponent>(animalEntity);
                painInfo.UnbearablePain = healthPercent * stats.PedUnbearablePain.Max;
                painInfo.PainRecoverySpeed = healthPercent * stats.PedPainRecoverySpeed.Max;
                painInfo.CurrentPainState = PainStates.NONE;
            }
        }
    }
}
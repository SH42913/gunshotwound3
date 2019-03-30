using System;
using System.Xml.Linq;
using GunshotWound2.Configs;
using GunshotWound2.GswWorld;
using GunshotWound2.Utils;
using Leopotam.Ecs;
using Rage;
using Rage.Native;

namespace GunshotWound2.Player.Systems
{
    [EcsInject]
    public class PlayerInitSystem : IEcsPreInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld = null;

        private readonly EcsFilter<LoadedConfigComponent> _loadedConfigs = null;
        private readonly EcsFilter<PlayerConfigComponent> _playerConfig = null;
        private readonly EcsFilter<GswPedComponent, NewPedMarkComponent> _newPeds = null;
        private readonly EcsFilter<GswPedComponent, PlayerMarkComponent> _playerPeds = null;

        private readonly GswLogger _logger;

        public PlayerInitSystem()
        {
            _logger = new GswLogger(typeof(PlayerInitSystem));
        }

        public void PreInitialize()
        {
            var config = _ecsWorld.AddComponent<PlayerConfigComponent>(GunshotWound2Script.StatsContainerEntity);
            config.PlayerEnabled = true;

            foreach (int i in _loadedConfigs)
            {
                XElement configRoot = _loadedConfigs.Components1[i].ElementRoot;
                XElement playerEnabledElement = configRoot.Element("PlayerEnabled");
                if (playerEnabledElement == null) continue;

                config.PlayerEnabled = playerEnabledElement.GetBool();
            }

#if DEBUG
            _logger.MakeLog(config.ToString());
#endif
            _logger.MakeLog("PlayerConfig is loaded!");
        }

        public void Run()
        {
            if (_playerConfig.IsEmpty())
            {
                throw new Exception("Player system was not init!");
            }

            PlayerConfigComponent config = _playerConfig.Components1[0];
            foreach (int i in _playerPeds)
            {
                Ped ped = _playerPeds.Components1[i].ThisPed;
                int entity = _playerPeds.Entities[i];
                if (Game.LocalPlayer.Character.Equals(ped)) continue;

                _ecsWorld.RemoveComponent<PlayerMarkComponent>(entity);
#if DEBUG
                _logger.MakeLog($"PlayerMark removed from ped {entity.GetEntityName()}, 'cause different characters");
#endif
            }

            foreach (int i in _newPeds)
            {
                Ped ped = _newPeds.Components1[i].ThisPed;
                int entity = _newPeds.Entities[i];
                if (!ped.IsLocalPlayer) continue;

                if (config.PlayerEnabled)
                {
                    _ecsWorld.AddComponent<PlayerMarkComponent>(entity);
                    NativeFunction.Natives.SET_PLAYER_HEALTH_RECHARGE_MULTIPLIER(Game.LocalPlayer, 0f);
#if DEBUG
                    _logger.MakeLog($"Ped {entity.GetEntityName()} was marked as player");
#endif
                }
                else
                {
                    _ecsWorld.RemoveComponent<NewPedMarkComponent>(entity);
#if DEBUG
                    _logger.MakeLog($"NewPedMark removed from ped {entity.GetEntityName()}, 'cause player is disabled");
#endif
                }
            }

            _ecsWorld.ProcessDelayedUpdates();
            if (config.PlayerEnabled && _playerPeds.IsEmpty() && !Game.IsPaused && !Game.IsLoading)
            {
#if DEBUG
                _logger.MakeLog("No players found! PlayerPed will force create!");
#endif
                _ecsWorld.CreateEntityWith<ForceCreateGswPedEvent>().TargetPed = Game.LocalPlayer.Character;
            }
        }

        public void PreDestroy()
        {
        }
    }
}
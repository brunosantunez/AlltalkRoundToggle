using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using Microsoft.Extensions.Logging;

namespace alltalkRoundToggle
{
    public class AllTalkRoundToggle : BasePlugin
    {
        private const VoiceFlags AllTalkFlags = VoiceFlags.All | VoiceFlags.ListenAll;
        private readonly Dictionary<int, VoiceFlags> _originalVoiceFlags = new();
        private (ConVar Variable, string OriginalValue)[] _voiceConVars = [];
        private bool _roundEnded;

        public override string ModuleName => "Alltalk Round Toggle";
        public override string ModuleAuthor => "brun0z";
        public override string ModuleDescription => "Alltalk Match: ON at the end of the round and OFF at the start";
        public override string ModuleVersion => "1.1.0";

        public override void Load(bool hotReload)
        {
            ConfigInfo config = CreateOrLoadConfig();
            if (!config.GeneralConfig.Enabled)
            {
                Logger.LogInformation("AlltalkRoundToggle is disabled by configuration");
                return;
            }

            string[] voiceConVarNames = ["sv_alltalk", "sv_deadtalk", "sv_full_alltalk", "sv_talk_enemy_dead", "sv_talk_enemy_living"];
            (ConVar Variable, string OriginalValue)[] voiceConVars = new (ConVar Variable, string OriginalValue)[voiceConVarNames.Length];
            for (int index = 0; index < voiceConVarNames.Length; index++)
            {
                string name = voiceConVarNames[index];
                ConVar variable = ConVar.Find(name)
                    ?? throw new InvalidOperationException($"CS2 voice convar '{name}' was not found; verify the game server and CounterStrikeSharp versions.");
                voiceConVars[index] = (variable, variable.StringValue);
            }
            _voiceConVars = voiceConVars;

            RegisterEventHandler<EventRoundStart>(OnRoundStart, HookMode.Post);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd, HookMode.Post);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull, HookMode.Post);

            Logger.LogInformation("AlltalkRoundToggle loaded");
        }

        public override void Unload(bool hotReload)
        {
            RestoreVoiceFlags();
            foreach ((ConVar variable, string originalValue) in _voiceConVars)
            {
                variable.StringValue = originalValue;
            }
        }

        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            _roundEnded = false;
            RestoreVoiceFlags();
            foreach ((ConVar variable, _) in _voiceConVars)
            {
                variable.SetValue(false);
            }
            Logger.LogInformation("Round started; all talk disabled");
            return HookResult.Continue;
        }

        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            _roundEnded = true;
            foreach ((ConVar variable, _) in _voiceConVars)
            {
                variable.SetValue(true);
            }
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                EnableAllTalk(player);
            }

            Logger.LogInformation("Round ended; all talk enabled");
            return HookResult.Continue;
        }

        private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            if (_roundEnded && @event.Userid is { IsValid: true } player)
            {
                EnableAllTalk(player);
            }

            return HookResult.Continue;
        }

        private void EnableAllTalk(CCSPlayerController player)
        {
            int userId = player.UserId ?? throw new InvalidOperationException("Connected player has no user ID; cannot enable all talk.");
            _originalVoiceFlags.TryAdd(userId, player.VoiceFlags);
            player.VoiceFlags |= AllTalkFlags;
        }

        private void RestoreVoiceFlags()
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player.UserId is int userId && _originalVoiceFlags.TryGetValue(userId, out VoiceFlags original))
                {
                    player.VoiceFlags = (player.VoiceFlags & ~AllTalkFlags) | (original & AllTalkFlags);
                }
            }

            _originalVoiceFlags.Clear();
        }

        private ConfigInfo CreateOrLoadConfig()
        {
            string configPath = Path.Join(ModuleDirectory, "AlltalkRoundToggle.json");
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "{\n  \"General\": {\n    \"Enabled\": true\n  }\n}");
            }

            string json = File.ReadAllText(configPath);
            ConfigInfo config;
            try
            {
                config = JsonSerializer.Deserialize<ConfigInfo>(json)
                    ?? throw new InvalidDataException($"Configuration file '{configPath}' is empty.");
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Invalid configuration file '{configPath}': {ex.Message}", ex);
            }

            if (config.GeneralConfig is null)
            {
                throw new InvalidDataException($"Configuration file '{configPath}' must contain a General section.");
            }

            return config;
        }

        public class ConfigInfo
        {
            [JsonPropertyName("General")]
            public required GeneralConfig GeneralConfig { get; init; }
        }

        public class GeneralConfig
        {
            public required bool Enabled { get; init; }
        }
    }
}

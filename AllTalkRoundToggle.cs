using System;
using System.IO;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Core;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace alltalkRoundToggle
{
    public class AllTalkRoundToggle : BasePlugin
    {
        public override string ModuleName => "Alltalk Round Toggle";
        public override string ModuleAuthor => "brun0z";
        public override string ModuleDescription => "Alltalk Match: ON at the end of the round and OFF at the start";
        public override string ModuleVersion => "1.0.0";

        ConVar? sv_alltalk = null!;
        ConVar? sv_deadtalk = null!;
        ConVar? sv_full_alltalk = null!;
        ConVar? sv_talk_enemy_dead = null!;
        ConVar? sv_talk_enemy_living = null!;

        ConfigInfo _config = new();

        public override void Load(bool hotReload)
        {
            _config = CreateOrLoadConfig();

            if (!_config.GeneralConfig!.Enabled)
            {
                Console.WriteLine("[AlltalkRoundToggle] Plugin not enabled!");
                return;
            }

            sv_alltalk = ConVar.Find("sv_alltalk");
            sv_deadtalk = ConVar.Find("sv_deadtalk");
            sv_full_alltalk = ConVar.Find("sv_full_alltalk");
            sv_talk_enemy_dead = ConVar.Find("sv_talk_enemy_dead");
            sv_talk_enemy_living = ConVar.Find("sv_talk_enemy_living");

            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);

            Console.WriteLine("[AlltalkRoundToggle] Plugin loaded successfully!");
        }

        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            if (!_config.GeneralConfig!.Enabled) return HookResult.Continue;

            sv_alltalk?.SetValue(false);
            sv_deadtalk?.SetValue(false);
            sv_full_alltalk?.SetValue(false);
            sv_talk_enemy_dead?.SetValue(false);
            sv_talk_enemy_living?.SetValue(false);

            Console.WriteLine("[AlltalkRoundToggle] Round Start -> Alltalk OFF");

            return HookResult.Continue;
        }

        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            if (!_config.GeneralConfig!.Enabled) return HookResult.Continue;

            sv_alltalk?.SetValue(true);
            sv_deadtalk?.SetValue(true);
            sv_full_alltalk?.SetValue(true);
            sv_talk_enemy_dead?.SetValue(true);
            sv_talk_enemy_living?.SetValue(true);

            Console.WriteLine("[AlltalkRoundToggle] Round End -> Alltalk ON");

            return HookResult.Continue;
        }

        private ConfigInfo CreateOrLoadConfig()
        {
            var configDirectory = Path.Join(ModuleDirectory, "AlltalkRoundToggle.json");
            if (!File.Exists(configDirectory))
            {
                string newJson = GenerateNewConfig();
                File.WriteAllText(configDirectory, newJson);
                return JsonSerializer.Deserialize<ConfigInfo>(newJson)!;
            }

            string json = File.ReadAllText(configDirectory);
            return JsonSerializer.Deserialize<ConfigInfo>(json)!;
        }

        private string GenerateNewConfig()
        {
            var jObject = new JsonObject
            {
                ["General"] = new JsonObject
                {
                    ["Enabled"] = true
                }
            };

            string json = JsonSerializer.Serialize(jObject, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return json;
        }

        public class ConfigInfo
        {
            [JsonPropertyName("General")]
            public GeneralConfig? GeneralConfig { get; set; }
        }

        public class GeneralConfig
        {
            public bool Enabled { get; set; }
        }
    }
}

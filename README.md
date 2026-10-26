# AlltalkRoundToggle

## Español

Permite que los jugadores vivos y muertos de ambos equipos se escuchen al terminar cada ronda. Al comenzar la siguiente, restaura los permisos de voz de cada jugador y desactiva las cinco cvars de voz que controla. Los silencios individuales siguen vigentes.

### Requisitos

- Metamod:Source 2.X Dev Build 1467 o posterior.
- CounterStrikeSharp 1.0.378 o posterior, con runtime de .NET 10.
- SDK de .NET 10 para compilar.

### Instalación

1. Compilá con `dotnet build AlltalkRoundToggle.csproj -c Release`.
2. Copiá `bin/Release/net10.0/AlltalkRoundToggle.dll` a `game/csgo/addons/counterstrikesharp/plugins/AlltalkRoundToggle/`.
3. Reiniciá el servidor.

El plugin crea `AlltalkRoundToggle.json` junto a la DLL con `General.Enabled` en `true`. Cambialo a `false` para desactivarlo.

Si CounterStrikeSharp no carga después de una actualización de CS2, actualizá Metamod:Source y CounterStrikeSharp antes de probar el plugin.

## English

Allows living and dead players on both teams to hear each other at the end of each round. When the next round starts, it restores each player's voice permissions and disables the five voice cvars it controls. Individual mutes remain in effect.

### Requirements

- Metamod:Source 2.X Dev Build 1467 or newer.
- CounterStrikeSharp 1.0.378 or newer, with the .NET 10 runtime.
- .NET 10 SDK to build the plugin.

### Installation

1. Build with `dotnet build AlltalkRoundToggle.csproj -c Release`.
2. Copy `bin/Release/net10.0/AlltalkRoundToggle.dll` to `game/csgo/addons/counterstrikesharp/plugins/AlltalkRoundToggle/`.
3. Restart the server.

The plugin creates `AlltalkRoundToggle.json` next to the DLL with `General.Enabled` set to `true`. Set it to `false` to disable the plugin.

If CounterStrikeSharp fails to load after a CS2 update, update Metamod:Source and CounterStrikeSharp before testing the plugin.

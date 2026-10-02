# Content Warning

## Pack metadata
- **Game display name:** Content Warning
- **Crowd Control game ID:** `ContentWarning`
- **Connector type:** `SimpleTCPServerConnector`
- **Endpoint port:** `51337`
- **Mod framework:** BepInEx `5.4.2100`


Crowd Control PC effect-pack definition for the game.

## Connector and layout

- `ContentWarning.cs` registers the effects through `SimpleTCPPack<SimpleTCPServerConnector>`.
- `mod/` contains game-side material; `src/ContentWarning.csproj` and its solution define the build.
- `thunderstore/manifest.json` supplies package metadata.

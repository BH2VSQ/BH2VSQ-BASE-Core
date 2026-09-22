# BH2VSQ BASE Core

VRChat World UdonSharp base system for Unity 2022.3.22f1 and VRChat Worlds SDK 3.10.4. The working design baseline is V1.3 of the supplied specification.

The source covers player registry and persistence, local TOTP sessions, floors and areas, teleports and consent requests, XP, radio duty, admin controls, and instance broadcast notifications. The `BH2VSQ BASE` Unity Editor menu builds the configured core prefab, component prefabs, default data, demo scene, and `.unitypackage` from source.

**Current status:** the source implementation passes offline checks. Unity Editor import, UdonSharp compilation, generated prefab assets, multi-client tests, and package export await a valid Unity license on this machine. The generated files are not claimed to exist yet. See [verification](verification.md).

## Start here

1. Activate Unity 2022.3.22f1 in Unity Hub.
2. Run `tools/Bootstrap-VRChat.ps1` to restore the pinned VRChat packages, or let VRChat Creator Companion resolve `Packages/vpm-manifest.json`.
3. Open this project in Unity. Use **BH2VSQ BASE → Build Package**. The menu compiles UdonSharp and creates the prefabs and demo scene.
4. Use **BH2VSQ BASE → Validate Setup** on the scene instance after configuring it.
5. Run `tools/Build-Package.ps1` to build and export `Releases/BH2VSQ_BASE_Core.unitypackage` in batch mode after Unity licensing is ready.

Install the exported package into an existing VRChat Worlds project, drag `Assets/BH2VSQ_BASE/Prefabs/Core/BH2VSQ_BASE_Core.prefab` into the scene, move the default teleport points to real destinations, configure TOTP on the scene instance, then validate and test with at least two VRChat clients. Detailed steps: [installation](Assets/BH2VSQ_BASE/Documentation/INSTALL.md), [configuration](Assets/BH2VSQ_BASE/Documentation/CONFIGURATION.md), [prefabs](Assets/BH2VSQ_BASE/Documentation/PREFABS.md), [security](Assets/BH2VSQ_BASE/Documentation/SECURITY.md).

## Repository layout

- `Assets/BH2VSQ_BASE/Scripts`: runtime UdonSharp and authoring ScriptableObjects.
- `Assets/BH2VSQ_BASE/Editor`: generator, setup wizard, and validator.
- `Assets/BH2VSQ_BASE/Fonts`: Noto Sans SC font and OFL notice.
- `Assets/BH2VSQ_BASE/Audio`: generated local broadcast tones.
- `Tests`: offline C# syntax, runtime type check with stubs, and RFC 6238 vectors. These do not replace Unity/Udon compilation.

Runtime authentication and network data are client controlled in a VRChat world. Do not treat this plugin as a security boundary for real secrets, paid access, or sensitive data. See [security](Assets/BH2VSQ_BASE/Documentation/SECURITY.md).

Code is MIT licensed. The bundled Noto Sans SC font is under the separate SIL Open Font License in `Assets/BH2VSQ_BASE/Fonts/OFL.txt`.
